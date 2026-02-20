using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Models;
using JSCodeSandbox.Services;

namespace JSCodeSandbox.Application.Services
{
    public class ProvisioningEnvironmentsService : IProvisioningEnvironmentsService
    {
        private readonly IProvisioningEnvironmentsRepository _provisioningEnvironmentsRepository;

        public ProvisioningEnvironmentsService(IProvisioningEnvironmentsRepository provisioningEnvironmentsRepository)
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
