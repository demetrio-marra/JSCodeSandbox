namespace JSCodeSandbox.Exceptions
{
    public class CodeExecutionException : Exception
    {
        public int SandboxExitCode { get; }
        public string StandardErrorOutput { get; }

        public CodeExecutionException(int sandboxExitCode, string stdErr) : base($"Error executing JavaScript code. Exit code: {sandboxExitCode}, Error: {stdErr}")
        {
            SandboxExitCode = sandboxExitCode;
            StandardErrorOutput = stdErr;
        }

        public CodeExecutionException(int sandboxExitCode, string stdErr, Exception innerException) : base($"Error executing JavaScript code. Exit code: {sandboxExitCode}, Error: {stdErr}", innerException)
        {
            SandboxExitCode = sandboxExitCode;
            StandardErrorOutput = stdErr;
        }
    }
}
