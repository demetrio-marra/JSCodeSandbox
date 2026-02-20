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

        public Task DeleteEnvironmentAsync(string provisionedEnvironmentName)
        {
            throw new NotImplementedException();
        }

        public Task ProvisionEnvironmentAsync(CodeExecutionEnvironmentCreationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
