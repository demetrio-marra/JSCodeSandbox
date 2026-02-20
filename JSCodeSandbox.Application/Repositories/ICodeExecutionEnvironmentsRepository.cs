using JSCodeSandbox.Models;

namespace JSCodeSandbox.Application.Repositories
{
    public interface ICodeExecutionEnvironmentsRepository
    {
        /// <summary>
        /// Creates a new code execution environment. If an environment with the same name already exists, an error is thrown.
        /// </summary>
        /// <param name="environment">The code execution environment to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CreateAsync(CodeExecutionEnvironment environment);

        /// <summary>
        /// Get the code execution environment by its name. Returns null if not found.
        /// </summary>
        /// <param name="environmentName">The name of the environment to retrieve.</param>
        /// <returns>The code execution environment if found; otherwise, null.</returns>
        Task<CodeExecutionEnvironment?> GetAsync(string environmentName);

        /// <summary>
        /// Deletes the code execution environment with the specified name. If the environment does not exist, this method does nothing.
        /// </summary>
        /// <param name="environmentName">The name of the environment to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(string environmentName);
    }
}
