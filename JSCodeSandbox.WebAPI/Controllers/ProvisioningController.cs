using JSCodeSandbox.Models;
using JSCodeSandbox.Services;
using Microsoft.AspNetCore.Mvc;

namespace JSCodeSandbox.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        /// Provisions a new code execution environment.
        /// </summary>
        /// <param name="request">The request containing environment configuration details.</param>
        /// <returns>HTTP 201 Created if successful.</returns>
        /// <response code="201">Environment provisioned successfully.</response>
        /// <response code="400">Invalid request data or validation failure.</response>
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
        /// <param name="environmentName">The name of the environment to delete.</param>
        /// <returns>HTTP 204 No Content if successful.</returns>
        /// <response code="204">Environment deleted successfully.</response>
        /// <response code="400">Invalid environment name or validation failure.</response>
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
