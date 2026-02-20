using JSCodeSandbox.Models;

namespace JSCodeSandbox.Application.Repositories
{
    public interface ICodeExecutionEnvironmentsRepository
    {
        Task CreateAsync(CodeExecutionEnvironment environment);
        Task<CodeExecutionEnvironment?> GetAsync(string environmentName);
        Task DeleteAsync(string environmentName);
    }
}
