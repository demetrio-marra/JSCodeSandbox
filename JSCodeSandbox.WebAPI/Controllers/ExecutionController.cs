using JSCodeSandbox.WebAPI.Models;
using JSCodeSandbox.Services;
using Microsoft.AspNetCore.Mvc;
using JSCodeSandbox.Models;

namespace JSCodeSandbox.WebAPI.Controllers
{
    /// <summary>
    /// Handles JavaScript code execution requests within previously provisioned sandboxed environments.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
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
        /// Executes JavaScript code in a provisioned sandboxed environment.
        /// </summary>
        /// <remarks>
        /// Submits a JavaScript code snippet for execution inside the sandbox identified by
        /// <paramref name="environmentName"/>. The environment must have been previously provisioned
        /// via the Provisioning API.
        ///
        /// **Sample request:**
        ///
        ///     POST /api/Execution/SuperUsers
        ///     {
        ///         "codeToRun": "console.log('Hello from sandbox!');",
        ///         "userAgentId": "user-agent-12345"
        ///     }
        ///
        /// **Sample response (success):**
        ///
        ///     {
        ///         "id": "6847a3b2-9f1c-4e5d-b8a0-1234567890ab",
        ///         "isError": false,
        ///         "executionResult": "Hello from sandbox!"
        ///     }
        ///
        /// **Sample response (code error):**
        ///
        ///     {
        ///         "id": "6847a3b2-9f1c-4e5d-b8a0-1234567890ab",
        ///         "isError": true,
        ///         "executionResult": "ReferenceError: undefinedVar is not defined"
        ///     }
        ///
        /// **Sample 400 response — InvalidRequest (validation failure):**
        ///
        /// Returned when the request fails general validation, such as missing or empty required fields,
        /// or referencing a non-existent environment.
        ///
        ///     {
        ///         "errorType": "InvalidRequest",
        ///         "error": "Environment name cannot be empty."
        ///     }
        ///
        /// **Sample 400 response — CodeSyntaxError (syntax error in submitted code):**
        ///
        /// Returned when the submitted JavaScript code contains a syntax error detected before execution.
        ///
        ///     {
        ///         "errorType": "CodeSyntaxError",
        ///         "error": "SyntaxError: Unexpected token '}'"
        ///     }
        /// </remarks>
        /// <param name="environmentName">The unique name of the provisioned environment in which the code will be executed.</param>
        /// <param name="request">The request body containing the JavaScript code and the caller's user agent identifier.</param>
        /// <returns>A <see cref="CodeExecutionResult"/> containing the execution output or error details.</returns>
        /// <response code="200">Code executed successfully. The response body contains the execution output.</response>
        /// <response code="400">
        /// The request is invalid. The response body contains a <c>errorType</c> field indicating the failure reason:
        /// <list type="bullet">
        ///   <item><description><b>InvalidRequest</b> — A general validation failure such as missing or empty required fields, or a non-existent environment.</description></item>
        ///   <item><description><b>CodeSyntaxError</b> — The submitted JavaScript code contains a syntax error detected before execution.</description></item>
        /// </list>
        /// </response>
        [HttpPost("{environmentName}")]
        [ProducesResponseType(typeof(CodeExecutionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
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
