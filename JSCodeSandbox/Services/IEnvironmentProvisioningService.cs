using JSCodeSandbox.Models;

namespace JSCodeSandbox.Services
{
    public interface IEnvironmentProvisioningService
    {
        /// <summary>
        /// Provisions a new sandbox environment for the user based on the provided request details.
        /// This method is responsible for creating and configuring a new environment that can be used for code execution, ensuring isolation and resource management.
        /// </summary>
        /// <param name="request">The request containing details necessary for provisioning the environment, such as user ID and any specific configuration parameters.</param>
        /// <returns>A task that represents the asynchronous provisioning operation.</returns>
        Task ProvisionEnvironmentAsync(ProvisioningEnvironmentCreationRequest request);

        /// <summary>
        /// Asynchronously deletes a provisioning environment based on the specified name.
        /// </summary>
        /// <param name="provisionedEnvironmentName">The name of the environment to be deleted.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteEnvironmentAsync(string provisionedEnvironmentName);
    }
}
