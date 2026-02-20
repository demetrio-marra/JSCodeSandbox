namespace JSCodeSandbox.Models
{
    public class CodeExecutionResult
    {
        /// <summary>
        /// Indicates whether the code execution resulted in an error.
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// The output of the code execution. If IsError is true, this will contain the error message. Otherwise, it will contain the standard output of the code execution.
        /// </summary>
        public string ExecutionResult { get; set; } = string.Empty;
    }
}
