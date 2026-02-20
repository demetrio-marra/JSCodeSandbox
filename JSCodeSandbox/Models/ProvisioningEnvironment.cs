namespace JSCodeSandbox.Models
{
    public class ProvisioningEnvironment
    {
        public string EnvironmentName { get; set; } = string.Empty;
        public Dictionary<string, string> BackendUrls { get; set; } = new Dictionary<string, string>();
        public string CodeImplementation { get; set; } = string.Empty;
    }
}
