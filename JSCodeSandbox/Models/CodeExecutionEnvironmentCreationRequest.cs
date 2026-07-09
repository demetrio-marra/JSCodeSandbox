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
        /// Examples of environment names include logical groupings such as "AccountsService".
        /// </summary>
        /// <example>AccountsService</example>
        [Required(ErrorMessage = "EnvironmentName is required.")]
        public string EnvironmentName { get; set; } = string.Empty;

        /// <summary>
        /// A dictionary of backend services that the sandboxed code will be allowed to call.
        /// Each entry maps a logical backend name (key) to its URL (value).
        /// These URLs are injected into the sandbox runtime so that executed code can make HTTP calls to them.
        /// </summary>
        /// <example>{"accountsServiceUrl": "https://api.mycompany.com/accounts"}</example>
        [Required(ErrorMessage = "BackendUrls is required.")]
        public Dictionary<string, string> BackendUrls { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The JavaScript bootstrap code that will be deployed into the provisioned environment.
        /// This typically includes helper functions, library imports, or framework initialization code
        /// that runs before any user-submitted code is executed.
        /// Follow the guidelines on https://github.com/demetrio-marra/JSCodeSandbox
        /// </summary>
        /// <example>
        /// const axios = require('axios');
        /// 
        /// // Module-level state populated by Initialize
        /// let serverUrl = null;
        /// let httpClient = null;
        /// 
        /// // 1. **Initialize Function**: Async function with exactly 2 parameters
        /// async function Initialize(agentId, backends) {
        ///     if (!backends || typeof backends !== 'object') {
        ///         throw new Error('backends parameter is required and must be an object');
        ///     }
        ///     serverUrl = backends['accountsServiceUrl'];
        ///     if (!serverUrl) {
        ///         throw new Error('accountsServiceUrl backend URL not found in backends configuration');
        ///     }
        /// 
        ///     // Pre-configure an axios instance bound to the service base URL
        ///     httpClient = axios.create({
        ///         baseURL: serverUrl,
        ///         timeout: 10000,
        ///         headers: {
        ///             'Content-Type': 'application/json'
        ///         }
        ///     });
        /// }
        /// 
        /// // 2. **Deinitialize Function**: Async, parameterless cleanup function
        /// async function Deinitialize() {
        ///     // Cleanup resources
        ///     httpClient = null;
        ///     serverUrl = null;
        /// }
        /// 
        /// /**
        ///  * @endowment
        ///  */
        /// async function findCustomerByName(params = {}) {
        ///     let { searchTerm } = params;
        /// 
        ///     if (!httpClient) {
        ///         throw new Error('Service not initialized. Call Initialize() before use.');
        ///     }
        ///     if (!searchTerm) {
        ///         throw new Error('searchTerm is required');
        ///     }
        /// 
        ///     try {
        ///         const response = await httpClient.get('/customers', {
        ///             params: { name: searchTerm }
        ///         });
        ///         return response.data;
        ///     } catch (error) {
        ///         if (error.response) {
        ///             // Server responded with a non-2xx status
        ///             throw new Error(
        ///                 `findCustomerByName failed: ${error.response.status} ${error.response.statusText}`
        ///             );
        ///         } else if (error.request) {
        ///             // Request was made but no response received
        ///             throw new Error(`findCustomerByName failed: no response from ${serverUrl}`);
        ///         } else {
        ///             throw new Error(`findCustomerByName failed: ${error.message}`);
        ///         }
        ///     }
        /// }
        /// 
        /// // any other private functions
        /// </example>
        [Required(ErrorMessage = "CodeImplementation is required.")]
        public string CodeImplementation { get; set; } = string.Empty;

        /// <summary>
        /// The full content of a <c>package.json</c> file that declares the Node.js dependencies for the environment.
        /// During provisioning, <c>npm install</c> is executed using this manifest to set up the required packages.
        /// If no dependencies are required leave it null.
        /// </summary>
        /// <example>{"dependencies": {"axios": "^1.6.0"}}</example>
        public string? PackageJson { get; set; }
    }
}
