using JSCodeSandbox.Models;
using JSCodeSandbox.Services;
using Microsoft.AspNetCore.Mvc;

namespace JSCodeSandbox.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExecutionController : ControllerBase
    {
        private readonly ICodeExecutionService _codeExecutionService;
        private readonly ILogger<ExecutionController> _logger;

        public ExecutionController(
            ICodeExecutionService codeExecutionService,
            ILogger<ExecutionController> logger)
        {
            _codeExecutionService = codeExecutionService;
            _logger = logger;
        }

        /// <summary>
        /// Executes JavaScript code in a provisioned environment.
        /// </summary>
        /// <param name="environmentName">The name of the provisioned environment to execute code in.</param>
        /// <param name="request">The request containing the code to execute and user agent information.</param>
        /// <returns>The result of the code execution.</returns>
        /// <response code="200">Code executed successfully.</response>
        /// <response code="400">Invalid request data or validation failure.</response>
        [HttpPost("{environmentName}")]
        [ProducesResponseType(typeof(CodeExecutionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExecuteCode(
            [FromRoute] string environmentName,
            [FromBody] CodeExecutionRequest request)
        {
            _logger.LogInformation("Executing code in environment: {EnvironmentName} for user: {UserAgentId}", 
                environmentName, request?.UserAgentId);
            
            var result = await _codeExecutionService.ExecuteCodeAsync(request, environmentName);
            
            return Ok(result);
        }
    }
}
