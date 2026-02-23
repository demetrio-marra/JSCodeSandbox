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

        public CodeExecutionService(ICodeExecutionEnvironmentsRepository environmentsRepository, ISandboxService sandboxService)
        {
            _environmentsRepository = environmentsRepository;
            _sandboxService = sandboxService;
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

            await _sandboxService.ProvisionAsync(environment.EnvironmentName, environment.CodeImplementation);

            try
            {
                var ret = await _sandboxService.RunCodeAsync(environment.EnvironmentName, input.UserAgentId, input.CodeToRun, environment.BackendUrls);
                return new CodeExecutionResult
                {
                    IsError = false,
                    ExecutionResult = ret
                };
            }
            catch (Exception ex)
            {
                return new CodeExecutionResult
                {
                    IsError = true,
                    ExecutionResult = $"Error executing code: {ex.Message}"
                };
            }
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
