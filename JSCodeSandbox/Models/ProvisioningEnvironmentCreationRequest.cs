namespace JSCodeSandbox.Models
{
    public class ProvisioningEnvironmentCreationRequest
    {
        /// <summary>
        /// The unique identifier of the environment to be provisioned.
        /// The environment defines the capabilities and resources available for code execution, such as the runtime, libraries, and isolation level.
        /// This can be used to specify different types of environments (e.g., "SuperUsers", "StatisticsRetrievers", etc.) or to track the provisioning request.
        /// </summary>
        public string EnvironmentName { get; set; } = string.Empty;

        /// <summary>
        /// The backends the sandbox code will have access to. The key is the backend name and the value is the backend URL.
        /// </summary>
        public Dictionary<string, string> BackendUrls { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The code implementation that will be executed in the provisioned environment. 
        /// This can include any necessary setup code, such as importing libraries, defining functions, or any other code that needs to be run before executing user-provided code.
        /// </summary>
        public string CodeImplementation { get; set; } = string.Empty;
    }
}
