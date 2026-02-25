using JSCodeSandbox.Models;

namespace JSCodeSandbox.Services
{
    /// <summary>
    /// Provides operations for querying code execution audit records.
    /// </summary>
    public interface ICodeExecutionsAuditService
    {
        /// <summary>
        /// Gets a code execution audit record by its ID.
        /// </summary>
        /// <param name="id">The ID of the code execution.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The code execution audit if found, otherwise null.</returns>
        Task<CodeExecutionAudit?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all code execution audit records with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of code execution audits with total count.</returns>
        Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets code execution audit records filtered by user agent ID with pagination.
        /// </summary>
        /// <param name="userAgentId">The user agent ID to filter by.</param>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of code execution audits for the specified user agent with total count.</returns>
        Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByUserAgentIdAsync(string userAgentId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets code execution audit records filtered by environment name with pagination.
        /// </summary>
        /// <param name="environmentName">The environment name to filter by.</param>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of code execution audits for the specified environment with total count.</returns>
        Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByEnvironmentNameAsync(string environmentName, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets code execution audit records filtered by execution error status with pagination.
        /// </summary>
        /// <param name="isExecutionError">Whether to filter for errors (true) or successful executions (false).</param>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of code execution audits matching the error status with total count.</returns>
        Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByErrorStatusAsync(bool isExecutionError, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a code execution audit record exists by its ID.
        /// </summary>
        /// <param name="id">The ID of the code execution.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the code execution audit exists, otherwise false.</returns>
        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    }
}
