using JSCodeSandbox.Models;
using JSCodeSandbox.Services;
using Microsoft.AspNetCore.Mvc;

namespace JSCodeSandbox.WebAPI.Controllers
{
    /// <summary>
    /// Manages the lifecycle of JavaScript code execution environments,
    /// including provisioning new sandboxed environments and deleting existing ones.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProvisioningController : ControllerBase
    {
        private readonly ICodeExecutionEnvironmentsProvisioningService _provisioningService;
        private readonly ILogger<ProvisioningController> _logger;

        public ProvisioningController(
            ICodeExecutionEnvironmentsProvisioningService provisioningService,
            ILogger<ProvisioningController> logger)
        {
            _provisioningService = provisioningService;
            _logger = logger;
        }

        /// <summary>
        /// Provisions a new JavaScript code execution environment.
        /// </summary>
        /// <remarks>
        /// Creates a new sandboxed environment with the specified configuration.
        /// The environment will be available for code execution via the Execution API
        /// once provisioning completes.
        ///
        /// **Sample request:**
        ///
        ///     POST /api/Provisioning
        ///     {
        ///         "environmentName": "SuperUsers",
        ///         "backendUrls": {
        ///             "userService": "https://api.example.com/users",
        ///             "orderService": "https://api.example.com/orders"
        ///         },
        ///         "codeImplementation": "const axios = require('axios'); async function fetchData(url) { return (await axios.get(url)).data; }",
        ///         "packageJson": "{\"name\": \"sandbox-env\", \"version\": \"1.0.0\", \"dependencies\": {\"axios\": \"^1.6.0\"}}"
        ///     }
        ///
        /// **Sample 400 response (validation failure):**
        ///
        ///     {
        ///         "errorType": "InvalidRequest",
        ///         "error": "EnvironmentName cannot be empty."
        ///     }
        /// </remarks>
        /// <param name="request">The request body containing the environment name, backend URLs, bootstrap code, and package.json manifest.</param>
        /// <returns>HTTP 201 Created with a Location header pointing to the provisioned environment.</returns>
        /// <response code="201">Environment provisioned successfully.</response>
        /// <response code="400">The request is invalid. Possible causes: missing required fields, duplicate environment name, or malformed package.json.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProvisionEnvironment([FromBody] CodeExecutionEnvironmentCreationRequest request)
        {
            _logger.LogInformation("Provisioning environment: {EnvironmentName}", request?.EnvironmentName);
            
            await _provisioningService.ProvisionEnvironmentAsync(request);
            
            return CreatedAtAction(nameof(ProvisionEnvironment), new { environmentName = request.EnvironmentName }, null);
        }

        /// <summary>
        /// Deletes an existing code execution environment.
        /// </summary>
        /// <remarks>
        /// Permanently removes the sandboxed environment identified by <paramref name="environmentName"/>.
        /// All associated resources (container, dependencies, configuration) are cleaned up.
        /// After deletion, the environment name can be reused for a new provisioning request.
        ///
        /// **Sample request:**
        ///
        ///     DELETE /api/Provisioning/SuperUsers
        ///
        /// **Sample 400 response (validation failure):**
        ///
        ///     {
        ///         "errorType": "InvalidRequest",
        ///         "error": "Environment 'SuperUsers' does not exist."
        ///     }
        /// </remarks>
        /// <param name="environmentName">The unique name of the environment to delete.</param>
        /// <returns>HTTP 204 No Content if the environment was deleted successfully.</returns>
        /// <response code="204">Environment deleted successfully. No content is returned.</response>
        /// <response code="400">The request is invalid. Possible causes: empty environment name or environment does not exist.</response>
        [HttpDelete("{environmentName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEnvironment([FromRoute] string environmentName)
        {
            _logger.LogInformation("Deleting environment: {EnvironmentName}", environmentName);
            
            await _provisioningService.DeleteEnvironmentAsync(environmentName);
            
            return NoContent();
        }
    }
}
