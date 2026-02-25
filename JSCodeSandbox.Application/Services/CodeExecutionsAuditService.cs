using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Exceptions;
using JSCodeSandbox.Models;
using JSCodeSandbox.Services;

namespace JSCodeSandbox.Application.Services
{
    public class CodeExecutionsAuditService : ICodeExecutionsAuditService
    {
        public const int MaxPageSize = 50;

        private readonly ICodeExecutionsAuditRepository _auditRepository;

        public CodeExecutionsAuditService(ICodeExecutionsAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }

        public async Task<CodeExecutionAudit?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ValidationException("Code execution audit ID cannot be null or empty.");
            }

            return await _auditRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            ValidatePaginationParameters(pageNumber, pageSize);

            return await _auditRepository.GetAllAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByUserAgentIdAsync(string userAgentId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userAgentId))
            {
                throw new ValidationException("User agent ID cannot be null or empty.");
            }

            ValidatePaginationParameters(pageNumber, pageSize);

            return await _auditRepository.GetByUserAgentIdAsync(userAgentId, pageNumber, pageSize, cancellationToken);
        }

        public async Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByEnvironmentNameAsync(string environmentName, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(environmentName))
            {
                throw new ValidationException("Environment name cannot be null or empty.");
            }

            ValidatePaginationParameters(pageNumber, pageSize);

            return await _auditRepository.GetByEnvironmentNameAsync(environmentName, pageNumber, pageSize, cancellationToken);
        }

        public async Task<(IEnumerable<CodeExecutionAudit> Items, int TotalCount)> GetByErrorStatusAsync(bool isExecutionError, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            ValidatePaginationParameters(pageNumber, pageSize);

            return await _auditRepository.GetByErrorStatusAsync(isExecutionError, pageNumber, pageSize, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ValidationException("Code execution audit ID cannot be null or empty.");
            }

            return await _auditRepository.ExistsAsync(id, cancellationToken);
        }

        private void ValidatePaginationParameters(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                throw new ValidationException("Page number must be greater than or equal to 1.");
            }

            if (pageSize < 1)
            {
                throw new ValidationException("Page size must be greater than or equal to 1.");
            }

            if (pageSize > MaxPageSize)
            {
                throw new ValidationException($"Page size must not exceed {MaxPageSize}.");
            }
        }
    }
}
