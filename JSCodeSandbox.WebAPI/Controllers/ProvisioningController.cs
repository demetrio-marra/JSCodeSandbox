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
        /// **Payload fields:**
        ///
        /// - `environmentName`: The unique name of the environment to be provisioned. Used to identify the environment across provisioning, execution, and deletion operations.
        /// - `backendUrls`: A dictionary of backend services that the sandboxed code will be allowed to call. Each entry maps a logical backend name to its URL.
        /// - `codeImplementation`: The JavaScript bootstrap code that will be deployed into the provisioned environment, including helper functions, library imports, or framework initialization code that runs before any user-submitted code is executed.
        /// - `packageJson`: The full content of a `package.json` file that declares the Node.js dependencies for the environment. If no dependencies are required, leave it `null`.
        ///
        /// **Sample request:**
        ///
        ///     POST /api/Provisioning
        ///     {
        ///         "environmentName": "AccountsService",
        ///         "backendUrls": {
        ///             "accountsServiceUrl": "https://api.mycompany.com/accounts"
        ///         },
        ///         "codeImplementation": "const axios = require('axios');\r\n\r\n// Module-level state populated by Initialize\r\nlet serverUrl = null;\r\nlet httpClient = null;\r\n\r\n// 1. **Initialize Function**: Async function with exactly 2 parameters\r\nasync function Initialize(agentId, backends) {\r\n    if (!backends || typeof backends !== 'object') {\r\n        throw new Error('backends parameter is required and must be an object');\r\n    }\r\n    serverUrl = backends['accountsServiceUrl'];\r\n    if (!serverUrl) {\r\n        throw new Error('accountsServiceUrl backend URL not found in backends configuration');\r\n    }\r\n\r\n    // Pre-configure an axios instance bound to the service base URL\r\n    httpClient = axios.create({\r\n        baseURL: serverUrl,\r\n        timeout: 10000,\r\n        headers: {\r\n            'Content-Type': 'application/json'\r\n        }\r\n    });\r\n}\r\n\r\n// 2. **Deinitialize Function**: Async, parameterless cleanup function\r\nasync function Deinitialize() {\r\n    // Cleanup resources\r\n    httpClient = null;\r\n    serverUrl = null;\r\n}\r\n\r\n/**\r\n * @endowment\r\n */\r\nasync function findCustomerByName(params = {}) {\r\n    let { searchTerm } = params;\r\n\r\n    if (!httpClient) {\r\n        throw new Error('Service not initialized. Call Initialize() before use.');\r\n    }\r\n    if (!searchTerm) {\r\n        throw new Error('searchTerm is required');\r\n    }\r\n\r\n    try {\r\n        const response = await httpClient.get('/customers', {\r\n            params: { name: searchTerm }\r\n        });\r\n        return response.data;\r\n    } catch (error) {\r\n        if (error.response) {\r\n            // Server responded with a non-2xx status\r\n            throw new Error(\r\n                `findCustomerByName failed: ${error.response.status} ${error.response.statusText}`\r\n            );\r\n        } else if (error.request) {\r\n            // Request was made but no response received\r\n            throw new Error(`findCustomerByName failed: no response from ${serverUrl}`);\r\n        } else {\r\n            throw new Error(`findCustomerByName failed: ${error.message}`);\r\n        }\r\n    }\r\n}\r\n\r\n// any other private functions",
        ///         "packageJson": "{\"dependencies\": {\"axios\": \"^1.6.0\"}}"
        ///     }
        ///
        /// **Sample 400 response (validation failure):**
        ///
        ///     {
        ///         "errorType": "InvalidRequest",
        ///         "error": "EnvironmentName cannot be empty."
        ///     }
        /// </remarks>
        /// <param name="request">The request body containing the environment name, allowed backend URLs, JavaScript bootstrap code, and optional `package.json` manifest used to provision the environment.</param>
        /// <returns>HTTP 201 Created with a Location header pointing to the provisioned environment.</returns>
        /// <response code="201">Environment provisioned successfully.</response>
        /// <response code="400">The request is invalid. Possible causes: missing required fields, duplicate environment name, or malformed package.json.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProvisionEnvironment([FromBody] CodeExecutionEnvironmentCreationRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation("Provisioning environment: {EnvironmentName}", request.EnvironmentName);
            
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

