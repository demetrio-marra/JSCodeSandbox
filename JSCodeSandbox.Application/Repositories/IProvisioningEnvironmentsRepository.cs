using JSCodeSandbox.Models;

namespace JSCodeSandbox.Application.Repositories
{
    public interface IProvisioningEnvironmentsRepository
    {
        Task CreateAsync(ProvisioningEnvironment environment);
        Task<ProvisioningEnvironment?> GetAsync(string environmentName);
        Task DeleteAsync(string environmentName);
    }
}
