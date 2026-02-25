using System.ComponentModel.DataAnnotations;

namespace JSCodeSandbox.Models
{
    /// <summary>
    /// Represents a request to provision a new JavaScript code execution environment.
    /// An environment encapsulates the runtime configuration, backend access, dependencies,
    /// and bootstrap code required to execute sandboxed JavaScript.
    /// </summary>
    public class CodeExecutionEnvironmentCreationRequest
    {
        /// <summary>
        /// The unique name of the environment to be provisioned.
        /// Used to identify the environment across provisioning, execution, and deletion operations.
        /// Examples of environment names include logical groupings such as "SuperUsers" or "StatisticsRetrievers".
        /// </summary>
        /// <example>SuperUsers</example>
        [Required(ErrorMessage = "EnvironmentName is required.")]
        public string EnvironmentName { get; set; } = string.Empty;

        /// <summary>
        /// A dictionary of backend services that the sandboxed code will be allowed to call.
        /// Each entry maps a logical backend name (key) to its URL (value).
        /// These URLs are injected into the sandbox runtime so that executed code can make HTTP calls to them.
        /// </summary>
        /// <example>{"userService": "https://api.example.com/users", "orderService": "https://api.example.com/orders"}</example>
        [Required(ErrorMessage = "BackendUrls is required.")]
        public Dictionary<string, string> BackendUrls { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The JavaScript bootstrap code that will be deployed into the provisioned environment.
        /// This typically includes helper functions, library imports, or framework initialization code
        /// that runs before any user-submitted code is executed.
        /// Follow the guidelines on https://github.com/demetrio-marra/JSCodeSandbox
        /// </summary>
        /// <example>const axios = require('axios'); async function fetchData(url) { return (await axios.get(url)).data; }</example>
        [Required(ErrorMessage = "CodeImplementation is required.")]
        public string CodeImplementation { get; set; } = string.Empty;

        /// <summary>
        /// The full content of a <c>package.json</c> file that declares the Node.js dependencies for the environment.
        /// During provisioning, <c>npm install</c> is executed using this manifest to set up the required packages.
        /// </summary>
        /// <example>{"name": "sandbox-env", "version": "1.0.0", "dependencies": {"axios": "^1.6.0"}}</example>
        [Required(ErrorMessage = "PackageJson is required.")]
        public string PackageJson { get; set; } = string.Empty;
    }
}
