using JSCodeSandbox.Models;
using JSCodeSandbox.Application.Services;
using JSCodeSandbox.Services;
using JSCodeSandbox.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace JSCodeSandbox.WebAPI.Controllers
{
    /// <summary>
    /// Provides read-only access to code execution audit records,
    /// including querying by ID, user agent, environment name, and error status with pagination support.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuditController : ControllerBase
    {
        private readonly ICodeExecutionsAuditService _auditService;
        private readonly ILogger<AuditController> _logger;

        public AuditController(
            ICodeExecutionsAuditService auditService,
            ILogger<AuditController> logger)
        {
            _auditService = auditService;
            _logger = logger;
        }

        /// <summary>
        /// Gets a code execution audit record by its ID.
        /// </summary>
        /// <remarks>
        /// Retrieves a single code execution audit record identified by <paramref name="id"/>.
        ///
        /// **Sample request:**
        ///
        ///     GET /api/Audit/6847a3b2-9f1c-4e5d-b8a0-1234567890ab
        ///
        /// **Sample 404 response:**
        ///
        /// Returned when the specified audit record does not exist.
        ///
        /// **Sample 400 response (validation failure):**
        ///
        ///     {
        ///         "errorType": "InvalidRequest",
        ///         "error": "Code execution audit ID cannot be null or empty."
        ///     }
        /// </remarks>
        /// <param name="id">The unique identifier of the audit record.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The code execution audit record if found.</returns>
        /// <response code="200">The audit record was found and returned.</response>
        /// <response code="400">The request is invalid (e.g., empty ID).</response>
        /// <response code="404">No audit record was found with the specified ID.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CodeExecutionAudit), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting audit record by ID: {Id}", id);

            var audit = await _auditService.GetByIdAsync(id, cancellationToken);
            if (audit == null)
            {
                return NotFound();
            }

            return Ok(audit);
        }

        /// <summary>
        /// Gets all code execution audit records with pagination.
        /// </summary>
        /// <remarks>
        /// Retrieves a paginated list of all code execution audit records.
        ///
        /// **Sample request:**
        ///
        ///     GET /api/Audit?pageNumber=1&amp;pageSize=10
        ///
        /// **Sample 400 response (validation failure):**
        ///
        ///     {
        ///         "errorType": "InvalidRequest",
        ///         "error": "Page number must be greater than or equal to 1."
        ///     }
        /// </remarks>
        /// <param name="pageNumber">The page number to retrieve (1-based). Default is 1.</param>
        /// <param name="pageSize">The number of items per page. Default is 10.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of audit records with total count.</returns>
        /// <response code="200">The paginated list of audit records.</response>
        /// <response code="400">The request is invalid (e.g., invalid pagination parameters).</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            pageSize = Math.Min(pageSize, CodeExecutionsAuditService.MaxPageSize);

            _logger.LogInformation("Getting all audit records. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

            var (items, totalCount) = await _auditService.GetAllAsync(pageNumber, pageSize, cancellationToken);

            return Ok(new { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize });
        }

        /// <summary>
        /// Gets code execution audit records filtered by user agent ID with pagination.
        /// </summary>
        /// <remarks>
        /// Retrieves a paginated list of code execution audit records for a specific user agent.
        ///
        /// **Sample request:**
        ///
        ///     GET /api/Audit/by-user-agent/user-agent-12345?pageNumber=1&amp;pageSize=10
        ///
        /// **Sample 400 response (validation failure):**
        ///
        ///     {
        ///         "errorType": "InvalidRequest",
        ///         "error": "User agent ID cannot be null or empty."
        ///     }
        /// </remarks>
        /// <param name="userAgentId">The user agent ID to filter by.</param>
        /// <param name="pageNumber">The page number to retrieve (1-based). Default is 1.</param>
        /// <param name="pageSize">The number of items per page. Default is 10.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of audit records for the specified user agent.</returns>
        /// <response code="200">The paginated list of audit records for the specified user agent.</response>
        /// <response code="400">The request is invalid (e.g., empty user agent ID or invalid pagination parameters).</response>
        [HttpGet("by-user-agent/{userAgentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByUserAgentId(
            [FromRoute] string userAgentId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            pageSize = Math.Min(pageSize, CodeExecutionsAuditService.MaxPageSize);

            _logger.LogInformation("Getting audit records by user agent ID: {UserAgentId}. Page: {PageNumber}, Size: {PageSize}", 
                userAgentId, pageNumber, pageSize);

            var (items, totalCount) = await _auditService.GetByUserAgentIdAsync(userAgentId, pageNumber, pageSize, cancellationToken);

            return Ok(new { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize });
        }

        /// <summary>
        /// Gets code execution audit records filtered by environment name with pagination.
        /// </summary>
        /// <remarks>
        /// Retrieves a paginated list of code execution audit records for a specific environment.
        ///
        /// **Sample request:**
        ///
        ///     GET /api/Audit/by-environment/SuperUsers?pageNumber=1&amp;pageSize=10
        ///
        /// **Sample 400 response (validation failure):**
        ///
        ///     {
        ///         "errorType": "InvalidRequest",
        ///         "error": "Environment name cannot be null or empty."
        ///     }
        /// </remarks>
        /// <param name="environmentName">The environment name to filter by.</param>
        /// <param name="pageNumber">The page number to retrieve (1-based). Default is 1.</param>
        /// <param name="pageSize">The number of items per page. Default is 10.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of audit records for the specified environment.</returns>
        /// <response code="200">The paginated list of audit records for the specified environment.</response>
        /// <response code="400">The request is invalid (e.g., empty environment name or invalid pagination parameters).</response>
        [HttpGet("by-environment/{environmentName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByEnvironmentName(
            [FromRoute] string environmentName,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            pageSize = Math.Min(pageSize, CodeExecutionsAuditService.MaxPageSize);

            _logger.LogInformation("Getting audit records by environment name: {EnvironmentName}. Page: {PageNumber}, Size: {PageSize}", 
                environmentName, pageNumber, pageSize);

            var (items, totalCount) = await _auditService.GetByEnvironmentNameAsync(environmentName, pageNumber, pageSize, cancellationToken);

            return Ok(new { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize });
        }

        /// <summary>
        /// Gets code execution audit records filtered by error status with pagination.
        /// </summary>
        /// <remarks>
        /// Retrieves a paginated list of code execution audit records filtered by whether they resulted in an error.
        ///
        /// **Sample request (get errors only):**
        ///
        ///     GET /api/Audit/by-error-status/true?pageNumber=1&amp;pageSize=10
        ///
        /// **Sample request (get successes only):**
        ///
        ///     GET /api/Audit/by-error-status/false?pageNumber=1&amp;pageSize=10
        ///
        /// **Sample 400 response (validation failure):**
        ///
        ///     {
        ///         "errorType": "InvalidRequest",
        ///         "error": "Page size must be greater than or equal to 1."
        ///     }
        /// </remarks>
        /// <param name="isExecutionError">Whether to filter for errors (true) or successful executions (false).</param>
        /// <param name="pageNumber">The page number to retrieve (1-based). Default is 1.</param>
        /// <param name="pageSize">The number of items per page. Default is 10.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of audit records matching the error status.</returns>
        /// <response code="200">The paginated list of audit records matching the error status filter.</response>
        /// <response code="400">The request is invalid (e.g., invalid pagination parameters).</response>
        [HttpGet("by-error-status/{isExecutionError}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByErrorStatus(
            [FromRoute] bool isExecutionError,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            pageSize = Math.Min(pageSize, CodeExecutionsAuditService.MaxPageSize);

            _logger.LogInformation("Getting audit records by error status: {IsExecutionError}. Page: {PageNumber}, Size: {PageSize}", 
                isExecutionError, pageNumber, pageSize);

            var (items, totalCount) = await _auditService.GetByErrorStatusAsync(isExecutionError, pageNumber, pageSize, cancellationToken);

            return Ok(new { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize });
        }

        /// <summary>
        /// Checks if a code execution audit record exists by its ID.
        /// </summary>
        /// <remarks>
        /// Returns whether an audit record with the specified <paramref name="id"/> exists.
        ///
        /// **Sample request:**
        ///
        ///     HEAD /api/Audit/6847a3b2-9f1c-4e5d-b8a0-1234567890ab
        ///
        /// Returns HTTP 200 if the record exists, or HTTP 404 if it does not.
        /// </remarks>
        /// <param name="id">The unique identifier of the audit record.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">The audit record exists.</response>
        /// <response code="400">The request is invalid (e.g., empty ID).</response>
        /// <response code="404">No audit record was found with the specified ID.</response>
        [HttpHead("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Exists([FromRoute] string id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking if audit record exists: {Id}", id);

            var exists = await _auditService.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
