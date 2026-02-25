using System.ComponentModel.DataAnnotations;

namespace JSCodeSandbox.Models
{
    /// <summary>
    /// Represents a request to execute JavaScript code in a provisioned sandboxed environment.
    /// </summary>
    public class CodeExecutionRequest
    {
        /// <summary>
        /// The JavaScript code to execute inside the sandboxed environment.
        /// Must be a valid JavaScript expression or script.
        /// Can of course be multiline and multiple statements, but for simplicity we treat it as a single string.
        /// </summary>
        /// <example>async function main() { let getStatisticsAPIResponse = await GetStatistics('product'); return getStatisticsAPIResponse.result; }</example>
        [Required(ErrorMessage = "CodeToRun is required.")]
        public string CodeToRun { get; set; } = string.Empty;

        /// <summary>
        /// A unique identifier for the user or agent requesting the code execution.
        /// Used for tracking, auditing, permission management, and associating execution results with a specific caller.
        /// </summary>
        /// <example>user-agent-12345</example>
        [Required(ErrorMessage = "UserAgentId is required.")]
        public string UserAgentId { get; set; } = string.Empty;
    }
}
