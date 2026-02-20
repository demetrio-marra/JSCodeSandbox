namespace JSCodeSandbox.Models
{
    public class CodeExecutionRequest
    {
        /// <summary>
        /// Gets or sets the code to execute.
        /// </summary>
        public string CodeToRun { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier for the user requesting code execution. This can be used for tracking, logging, or associating execution results with a specific user.
        /// </summary>
        public string UserAgentId { get; set; } = string.Empty;
    }
}
