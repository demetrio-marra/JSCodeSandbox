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
        /// <returns>A result containing the unique identifier of the provisioned environment, which can be used for subsequent operations like code execution.</returns>
        Task<ProvisioningEnvironmentCreationResult> ProvisionEnvironmentAsync(ProvisioningEnvironmentCreationRequest request);

        /// <summary>
        /// Asynchronously deletes a provisioning environment based on the specified request.
        /// </summary>
        /// <param name="request">The request containing details for the environment deletion.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteEnvironmentAsync(ProvisioningEnvironmentDeletionRequest request);
    }
}
