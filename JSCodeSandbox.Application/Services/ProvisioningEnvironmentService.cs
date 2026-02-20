using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Models;
using JSCodeSandbox.Services;

namespace JSCodeSandbox.Application.Services
{
    public class ProvisioningEnvironmentService : IProvisioningEnvironmentService
    {
        private readonly IProvisioningEnvironmentsRepository _provisioningEnvironmentsRepository;

        public ProvisioningEnvironmentService(IProvisioningEnvironmentsRepository provisioningEnvironmentsRepository)
        {
            _provisioningEnvironmentsRepository = provisioningEnvironmentsRepository;
        }

        public Task DeleteEnvironmentAsync(string provisionedEnvironmentName)
        {
            throw new NotImplementedException();
        }

        public Task ProvisionEnvironmentAsync(ProvisioningEnvironmentCreationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
