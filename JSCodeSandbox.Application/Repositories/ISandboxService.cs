namespace JSCodeSandbox.Application.Repositories
{
    public interface ISandboxService
    {
        /// <summary>
        /// Runs the provided JavaScript code in a sandboxed environment and returns the result as a string.
        /// The method takes the name of the environment to run the code in, a user agent ID for tracking purposes, and the JavaScript code to execute. 
        /// The implementation of this method should ensure that the code is executed securely in an isolated environment, preventing any potential harm to the host system. 
        /// The result returned can be either the standard output of the executed code or an error message if the execution fails.
        /// </summary>
        /// <param name="environmentName">The name of the environment to run the code in.</param>
        /// <param name="userAgentId">The user agent ID for tracking purposes.</param>
        /// <param name="code">The JavaScript code to execute.</param>
        /// <param name="backends">A dictionary of backend services that may be used during code execution, where the key is the backend name and the value is the backend URL or connection string.</param>
        /// <returns>The result of the code execution as a string.</returns>
        /// <remarks>Always call ProvisionAsync before calling this method.</remarks>
        Task<string> RunCodeAsync(string environmentName, string userAgentId, string code, Dictionary<string, string> backends);

        /// <summary>
        /// Provisions a new sandboxed environment with the specified name.
        /// This method is responsible for setting up the environment, including any necessary resources or configurations, to ensure that it is ready for executing JavaScript code securely.
        /// </summary>
        /// <param name="environmentName">The name of the environment to provision.</param>
        /// <param name="codeImplementation">The JavaScript code to execute in the provisioned environment.</param>
        /// <param name="packageJson">The package.json content that defines the Node.js dependencies for the environment.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ProvisionAsync(string environmentName, string codeImplementation, string packageJson);
    }
}
