using JSCodeSandbox.Models;

namespace JSCodeSandbox.Application.Repositories
{
    public interface ICodeExecutionsAuditRepository
    {
        /// <summary>
        /// Creates a new code execution record.
        /// </summary>
        /// <param name="codeExecution">The code execution to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created code execution.</returns>
        Task<CodeExecutionAudit> CreateAsync(CodeExecutionAudit codeExecution, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a code execution by its ID.
        /// </summary>
        /// <param name="id">The ID of the code execution.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The code execution if found, otherwise null.</returns>
        Task<CodeExecutionAudit?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all code executions with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of code executions with total count.</returns>
        Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets code executions filtered by user agent ID with pagination.
        /// </summary>
        /// <param name="userAgentId">The user agent ID to filter by.</param>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of code executions for the specified user agent with total count.</returns>
        Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByUserAgentIdAsync(string userAgentId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets code executions filtered by environment name with pagination.
        /// </summary>
        /// <param name="environmentName">The environment name to filter by.</param>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of code executions for the specified environment with total count.</returns>
        Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByEnvironmentNameAsync(string environmentName, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets code executions filtered by execution error status with pagination.
        /// </summary>
        /// <param name="isExecutionError">Whether to filter for errors (true) or successful executions (false).</param>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated collection of code executions matching the error status with total count.</returns>
        Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByErrorStatusAsync(bool isExecutionError, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
            
        /// <summary>
        /// Deletes a code execution by its ID.
        /// </summary>
        /// <param name="id">The ID of the code execution to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a code execution exists by its ID.
        /// </summary>
        /// <param name="id">The ID of the code execution.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the code execution exists, otherwise false.</returns>
        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    }
}
