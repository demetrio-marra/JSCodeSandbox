using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Exceptions;
using JSCodeSandbox.Models;
using JSCodeSandbox.Services;
using Esprima;
using Esprima.Ast;

namespace JSCodeSandbox.Application.Services
{
    public class CodeExecutionEnvironmentsProvisioningService : ICodeExecutionEnvironmentsProvisioningService
    {
        private readonly ICodeExecutionEnvironmentsRepository _provisioningEnvironmentsRepository;

        public CodeExecutionEnvironmentsProvisioningService(ICodeExecutionEnvironmentsRepository provisioningEnvironmentsRepository)
        {
            _provisioningEnvironmentsRepository = provisioningEnvironmentsRepository;
        }

        public async Task DeleteEnvironmentAsync(string provisionedEnvironmentName)
        {
            if (string.IsNullOrWhiteSpace(provisionedEnvironmentName))
            {
                throw new ValidationException("Environment name cannot be null or empty.");
            }

            await _provisioningEnvironmentsRepository.DeleteAsync(provisionedEnvironmentName);
        }

        public async Task ProvisionEnvironmentAsync(CodeExecutionEnvironmentCreationRequest request)
        {
            if (request == null)
            {
                throw new ValidationException("Request cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(request.EnvironmentName))
            {
                throw new ValidationException("Environment name cannot be null or empty.");
            }

            if (request.BackendUrls == null)
            {
                throw new ValidationException("Backend URLs cannot be null.");
            }

            if (!request.BackendUrls.Any())
            {
                throw new ValidationException("At least one backend URL must be provided.");
            }

            // validate each backend URL
            foreach (var url in request.BackendUrls)
            {
                var k = url.Key;
                if (string.IsNullOrWhiteSpace(k))
                {
                    throw new ValidationException("Backend URL key cannot be null or empty.");
                }

                var v = url.Value;
                if (string.IsNullOrWhiteSpace(v))
                {
                    throw new ValidationException("Backend URL cannot be null or empty.");
                }
                if (!Uri.IsWellFormedUriString(v, UriKind.Absolute))
                {
                    throw new ValidationException($"Invalid backend URL: {url}");
                }
            }

            if (string.IsNullOrWhiteSpace(request.CodeImplementation))
            {
                throw new ValidationException("Code implementation cannot be null or empty.");
            }

            var endowmentFunctions = await ValidateCodeImplementation(request.CodeImplementation);

            var functionsToBeExported = new List<string>(endowmentFunctions) { "Initialize", "Deinitialize" };
            var exportedFunctionsStatement = "module.exports = {" + string.Join(", ", functionsToBeExported) + "};";

            var completeCode = request.CodeImplementation + "\n\n" + exportedFunctionsStatement;

            var environment = new CodeExecutionEnvironment
            {
                EnvironmentName = request.EnvironmentName,
                BackendUrls = request.BackendUrls,
                CodeImplementation = completeCode,
            };

            await _provisioningEnvironmentsRepository.CreateAsync(environment);
        }

        /// <summary>
        /// Runs validation and returns the list of endowment functions defined in the code implementation. Validations include:
        /// - Presence of async 'Deinitialize' function with no parameters
        /// - Presence of async 'Initialize' function with parameters 'agentId' and 'backends'
        /// - Presence of at least one async function annotated with '@endowment' and a single 'params = {}' parameter
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        private async Task<List<string>> ValidateCodeImplementation(string code)
        {
            await Task.CompletedTask;

            // Parse the JavaScript code
            Script ast;
            try
            {
                var parserOptions = new ParserOptions
                {
                    Comments = true,
                    Tolerant = false
                };
                var parser = new JavaScriptParser(parserOptions);
                ast = parser.ParseScript(code);
            }
            catch (ParserException ex)
            {
                throw new ValidationException($"JavaScript syntax error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new ValidationException($"Failed to parse JavaScript code: {ex.Message}");
            }

            // Track required functions
            var hasDeinitialize = false;
            var hasInitialize = false;
            var endowmentFunctions = new List<string>();

            // Get all comments with their positions
            var commentMap = new Dictionary<int, List<SyntaxComment>>();
            if (ast.Comments != null)
            {
                foreach (var comment in ast.Comments)
                {
                    var commentEnd = comment.Range.End;
                    if (!commentMap.ContainsKey(commentEnd))
                    {
                        commentMap[commentEnd] = new List<SyntaxComment>();
                    }
                    commentMap[commentEnd].Add(comment);
                }
            }

            // Validate that all required functions are at the top scope
            foreach (var node in ast.Body)
            {
                if (node is not FunctionDeclaration funcDecl)
                {
                    continue;
                }

                var functionName = funcDecl.Id?.Name;
                if (string.IsNullOrEmpty(functionName))
                {
                    continue;
                }

                // Check if function is async
                var isAsync = funcDecl.Async;

                // Check for Deinitialize
                if (functionName == "Deinitialize")
                {
                    if (!isAsync)
                    {
                        throw new ValidationException("'Deinitialize' function must be async.");
                    }
                    if (funcDecl.Params.Count != 0)
                    {
                        throw new ValidationException("'Deinitialize' function must be parameterless.");
                    }
                    hasDeinitialize = true;
                }

                // Check for Initialize
                if (functionName == "Initialize")
                {
                    if (!isAsync)
                    {
                        throw new ValidationException("'Initialize' function must be async.");
                    }
                    if (funcDecl.Params.Count != 2)
                    {
                        throw new ValidationException("'Initialize' function must have exactly 2 parameters: agentId and backends.");
                    }
                    var param1 = funcDecl.Params[0] as Identifier;
                    var param2 = funcDecl.Params[1] as Identifier;
                    if (param1?.Name != "agentId" || param2?.Name != "backends")
                    {
                        throw new ValidationException("'Initialize' function must have signature 'async function Initialize(agentId, backends)'.");
                    }
                    hasInitialize = true;
                }

                // Check for @endowment functions
                // Look for @endowment in comments before this function
                var hasEndowmentAnnotation = false;
                var funcStart = funcDecl.Range.Start;
                
                foreach (var kvp in commentMap.Where(c => c.Key < funcStart))
                {
                    foreach (var comment in kvp.Value)
                    {
                        if (comment.Value.Contains("@endowment"))
                        {
                            hasEndowmentAnnotation = true;
                            break;
                        }
                    }
                    if (hasEndowmentAnnotation) break;
                }

                if (hasEndowmentAnnotation)
                {
                    if (!isAsync)
                    {
                        throw new ValidationException($"Endowment function '{functionName}' must be async.");
                    }
                    if (funcDecl.Params.Count != 1)
                    {
                        throw new ValidationException($"Endowment function '{functionName}' must have exactly one parameter.");
                    }

                    // Check if parameter has default value of {}
                    var param = funcDecl.Params[0];
                    if (param is AssignmentPattern assignmentPattern)
                    {
                        if (assignmentPattern.Left is Identifier paramId &&
                            paramId.Name == "params" &&
                            assignmentPattern.Right is ObjectExpression objExpr &&
                            objExpr.Properties.Count == 0)
                        {
                            endowmentFunctions.Add(functionName);
                        }
                        else
                        {
                            throw new ValidationException($"Endowment function '{functionName}' must have a single 'params = {{}}' parameter.");
                        }
                    }
                    else
                    {
                        throw new ValidationException($"Endowment function '{functionName}' must have a single 'params = {{}}' parameter with default value.");
                    }
                }
            }

            // Validate all requirements are met
            if (!hasDeinitialize)
            {
                throw new ValidationException(
                    "Code implementation must contain an async parameterless 'Deinitialize' function defined in the top scope.");
            }

            if (!hasInitialize)
            {
                throw new ValidationException(
                    "Code implementation must contain an async 'Initialize' function with signature 'async function Initialize(agentId, backends)'.");
            }

            if (endowmentFunctions.Count == 0)
            {
                throw new ValidationException(
                    "Code implementation must contain at least one async function with a single 'params = {{}}' parameter annotated with '@endowment'.");
            }

            return endowmentFunctions;
        }
    }
}
