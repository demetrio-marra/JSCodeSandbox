using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Exceptions;
using JSCodeSandbox.Models;
using JSCodeSandbox.Services;

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

            await ValidateCodeImplementation(request.CodeImplementation);

            var environment = new CodeExecutionEnvironment
            {
                EnvironmentName = request.EnvironmentName,
                BackendUrls = request.BackendUrls,
                CodeImplementation = request.CodeImplementation
            };

            await _provisioningEnvironmentsRepository.CreateAsync(environment);
        }

        private async Task ValidateCodeImplementation(string code)
        {
            // Implement code validation logic here, such as syntax checking or sandboxing rules.
            // This is a placeholder for demonstration purposes.
            await Task.CompletedTask;
        }
    }
}
