using JSCodeSandbox.Application.Repositories;
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
            await _provisioningEnvironmentsRepository.DeleteAsync(provisionedEnvironmentName);
        }

        public async Task ProvisionEnvironmentAsync(CodeExecutionEnvironmentCreationRequest request)
        {
            var environment = new CodeExecutionEnvironment
            {
                EnvironmentName = request.EnvironmentName,
                BackendUrls = request.BackendUrls,
                CodeImplementation = request.CodeImplementation
            };

            await _provisioningEnvironmentsRepository.CreateAsync(environment);
        }
    }
}
