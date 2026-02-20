using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Exceptions;
using JSCodeSandbox.Models;
using JSCodeSandbox.Services;
using Esprima;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSCodeSandbox.Application.Services
{
    public class CodeExecutionService : ICodeExecutionService
    {
        private readonly ICodeExecutionEnvironmentsRepository _environmentsRepository;

        public CodeExecutionService(ICodeExecutionEnvironmentsRepository environmentsRepository)
        {
            _environmentsRepository = environmentsRepository;
        }

        public async Task<CodeExecutionResult> ExecuteCodeAsync(CodeExecutionRequest input, string provisionedEnvironmentName)
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

            await ValidateJavaScriptSyntax(input.CodeToRun);

            throw new NotImplementedException();
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
