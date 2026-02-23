using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Exceptions;
using JSCodeSandbox.Models;
using JSCodeSandbox.Services;
using Esprima;

namespace JSCodeSandbox.Application.Services
{
    public class CodeExecutionService : ICodeExecutionService
    {
        private readonly ICodeExecutionEnvironmentsRepository _environmentsRepository;
        private readonly ISandboxService _sandboxService;
        private readonly ICodeExecutionsAuditRepository _codeExecutionsAuditRepository;

        public CodeExecutionService(ICodeExecutionEnvironmentsRepository environmentsRepository, 
            ISandboxService sandboxService,
            ICodeExecutionsAuditRepository codeExecutionsAuditRepository)
        {
            _environmentsRepository = environmentsRepository;
            _sandboxService = sandboxService;
            _codeExecutionsAuditRepository = codeExecutionsAuditRepository;
        }

        public async Task<CodeExecutionResult> ExecuteCodeAsync(CodeExecutionRequest input, string provisionedEnvironmentName)
        {
            ValidateRequest(input, provisionedEnvironmentName);

            await ValidateJavaScriptSyntax(input.CodeToRun);

            var environment = await _environmentsRepository.GetAsync(provisionedEnvironmentName);
            if (environment == null)
            {
                throw new ValidationException($"Provisioned environment '{provisionedEnvironmentName}' not found.");
            }

            try
            {
                // Always call the provision method. It creates or do nothing dependencies for the environment, so it's safe to call it every time.
                await _sandboxService.ProvisionAsync(environment.EnvironmentName, environment.CodeImplementation, environment.PackageJson);
            } 
            catch (Exception ex)
            {
                throw new InfrastructureError(GetType().Name, $"Failed to provision sandbox environment: {ex.Message}", ex);
            }

            var startTime = DateTime.UtcNow;    
            var isExecutionError = false;
            string executionResult = string.Empty;
            string executionId = string.Empty;
            try
            {
                var ret = await _sandboxService.RunCodeAsync(environment.EnvironmentName, input.UserAgentId, input.CodeToRun, environment.BackendUrls);
                executionResult = ret;
            }
            catch (InfrastructureError ex)
            {
                executionResult = ex.Message;
                isExecutionError = true;
                throw;
            }
            catch (CodeExecutionException ex)
            {
                executionResult = ex.Message;
                isExecutionError = true;
            }   
            catch (Exception ex)
            {
                executionResult = ex.Message;
                isExecutionError = true;
                throw new InfrastructureError(GetType().Name, $"Unexpected error during code execution: {ex.Message}", ex);
            }
            finally
            {
                var endTime = DateTime.UtcNow;

                var codeExecutionAudit = new CodeExecutionAudit
                {
                    UserAgentId = input.UserAgentId,
                    EnvironmentName = environment.EnvironmentName,
                    CodeToRun = input.CodeToRun,
                    CompletedOnUTC = endTime,
                    Hostname = Environment.MachineName,
                    IsExecutionError = isExecutionError,
                    ExecutionResult = executionResult,
                    StartedOnUTC = startTime
                };

                var savedAudit = await _codeExecutionsAuditRepository.CreateAsync(codeExecutionAudit);
                executionId = savedAudit.Id;
            }

            return new CodeExecutionResult
            {
                ExecutionResult = executionResult,
                IsError = isExecutionError,
                Id = executionId
            };
        }


        private void ValidateRequest(CodeExecutionRequest input, string provisionedEnvironmentName)
        {
            if (input == null)
            {
                throw new ValidationException("Request cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(input.CodeToRun))
            {
                throw new ValidationException("Code to run cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(input.UserAgentId))
            {
                throw new ValidationException("User agent ID cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(provisionedEnvironmentName))
            {
                throw new ValidationException("Provisioned environment name cannot be null or empty.");
            }
        }

        private async Task ValidateJavaScriptSyntax(string code)
        {
            await Task.CompletedTask;

            try
            {
                var parserOptions = new ParserOptions
                {
                    Tolerant = false
                };
                var parser = new JavaScriptParser(parserOptions);
                parser.ParseScript(code);
            }
            catch (ParserException ex)
            {
                throw new InvalidCodeToRunException($"JavaScript syntax error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidCodeToRunException($"Failed to parse JavaScript code: {ex.Message}");
            }
        }
    }
}
