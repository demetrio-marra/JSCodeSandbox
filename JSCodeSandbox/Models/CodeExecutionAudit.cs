namespace JSCodeSandbox.Models
{
    /// <summary>
    /// Represents a log of code execution, including the code that was run, the user agent information, the environment in which it was executed, and the results of the execution.
    /// </summary>
    public class CodeExecutionAudit
    {
        /// <summary>
        /// Gets or sets the unique identifier for the code execution.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets the date and time when the code execution was started, in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime StartedOnUTC { get; set; }

        /// <summary>
        /// Gets the date and time when the operation was completed, in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime CompletedOnUTC { get; set; }

        /// <summary>
        /// Gets the code that was executed. This is the JavaScript code that the user requested to run in the sandboxed environment.
        /// </summary>
        public string CodeToRun { get; set; } = string.Empty;

        /// <summary>
        /// Gets the user agent ID associated with the code execution. This is a unique identifier for the user or client that initiated the code execution request. It can be used for tracking and logging purposes to identify which user or client executed the code.
        /// </summary>
        public string UserAgentId { get; set; } = string.Empty;

        /// <summary>
        /// Gets the name of the environment in which the code was executed. This corresponds to the provisioned environment that was used to run the code. It can be used for tracking and logging purposes to identify which environment was used for the execution.
        /// </summary>
        public string EnvironmentName { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the code execution resulted in an error. If true, it means that there was an error during the execution of the code, and the ExecutionResult property will contain the error message. If false, it means that the code executed successfully, and the ExecutionResult property will contain the standard output of the code execution.
        /// </summary>
        public bool IsExecutionError { get; set; }

        /// <summary>
        /// Gets the result of the code execution. If IsExecutionError is false, this property contains the standard output produced by the executed code. If IsExecutionError is true, this property contains the error message or stack trace resulting from the execution failure. This allows for detailed logging and debugging of code execution attempts in the sandboxed environment.
        /// </summary>
        public string ExecutionResult { get; set; } = string.Empty;
    }
}
