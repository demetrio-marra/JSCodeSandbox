namespace JSCodeSandbox.Models
{
    /// <summary>
    /// Represents the result of a JavaScript code execution performed inside a sandboxed environment.
    /// Contains the execution output or error details, along with a unique identifier for audit and tracking purposes.
    /// </summary>
    public class CodeExecutionResult
    {
        /// <summary>
        /// A unique identifier assigned to this execution result.
        /// Can be used for audit trails, log correlation, and tracking individual executions.
        /// </summary>
        /// <example>6847a3b2-9f1c-4e5d-b8a0-1234567890ab</example>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the code execution resulted in an error.
        /// When <c>true</c>, the <see cref="ExecutionResult"/> property contains the error message.
        /// When <c>false</c>, it contains the standard output of the successful execution.
        /// </summary>
        /// <example>false</example>
        public bool IsError { get; set; }

        /// <summary>
        /// The output produced by the code execution.
        /// If <see cref="IsError"/> is <c>true</c>, this contains the error message or stack trace.
        /// If <see cref="IsError"/> is <c>false</c>, this contains the standard output (e.g., console.log output).
        /// </summary>
        /// <example>Hello from sandbox!</example>
        public string ExecutionResult { get; set; } = string.Empty;
    }
}
