namespace JSCodeSandbox.Application.Repositories
{
    public interface ISandboxService
    {
        Task<string> RunCode(string environmentBasePath, string userAgentId, string code);
    }
}
