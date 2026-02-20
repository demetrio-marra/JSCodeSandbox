using JSCodeSandbox.Models;

namespace JSCodeSandbox.Services
{
    public interface ICodeExecutionService
    {
        /// <summary>
        /// Runs the provided code in a sandboxed environment and returns the output.
        /// The provisionedEnvironmentId is used to identify which sandbox environment to use for execution, allowing for isolation and resource management.
        /// </summary>
        /// <param name="input">The input containing the code to execute and the user ID.</param>
        /// <param name="provisionedEnvironmentId">The ID of the provisioned environment to run the code in.</param>
        /// <returns>The output of the code execution.</returns>
        Task<CodeExecutionResult> ExecuteCodeAsync(CodeExecutionRequest input, string provisionedEnvironmentId);
    }
}
