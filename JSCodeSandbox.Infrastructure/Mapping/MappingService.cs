using JSCodeSandbox.Application.Models;
using JSCodeSandbox.Infrastructure.Entities;
using JSCodeSandbox.Models;

namespace JSCodeSandbox.Infrastructure.Mapping
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source) where TDestination : new();
        IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source) where TDestination : new();
    }

    public class MappingService : IMapper
    {
        public TDestination Map<TSource, TDestination>(TSource source) where TDestination : new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var destination = new TDestination();

            if (typeof(TSource) == typeof(CodeExecutionEnvironment) && typeof(TDestination) == typeof(CodeExecutionEnvironmentEntity))
            {
                var src = source as CodeExecutionEnvironment;
                var dest = destination as CodeExecutionEnvironmentEntity;
                if (src != null && dest != null)
                {
                    dest.EnvironmentName = src.EnvironmentName;
                    dest.BackendUrls = src.BackendUrls;
                    dest.CodeImplementation = src.CodeImplementation;
                    dest.PackageJson = src.PackageJson;
                    dest.EndowmentFunctions = src.EndowmentFunctions;
                }
            }
            else if (typeof(TSource) == typeof(CodeExecutionEnvironmentEntity) && typeof(TDestination) == typeof(CodeExecutionEnvironment))
            {
                var src = source as CodeExecutionEnvironmentEntity;
                var dest = destination as CodeExecutionEnvironment;
                if (src != null && dest != null)
                {
                    dest.EnvironmentName = src.EnvironmentName;
                    dest.BackendUrls = src.BackendUrls;
                    dest.CodeImplementation = src.CodeImplementation;
                    dest.PackageJson = src.PackageJson;
                    dest.EndowmentFunctions = src.EndowmentFunctions;
                }
            }
            else if (typeof(TSource) == typeof(CodeExecutionAudit) && typeof(TDestination) == typeof(CodeExecutionAuditEntity))
            {
                var src = source as CodeExecutionAudit;
                var dest = destination as CodeExecutionAuditEntity;
                if (src != null && dest != null)
                {
                    dest.Id = src.Id;
                    dest.StartedOnUTC = src.StartedOnUTC;
                    dest.CompletedOnUTC = src.CompletedOnUTC;
                    dest.CodeToRun = src.CodeToRun;
                    dest.UserAgentId = src.UserAgentId;
                    dest.EnvironmentName = src.EnvironmentName;
                    dest.IsExecutionError = src.IsExecutionError;
                    dest.ExecutionResult = src.ExecutionResult;
                    dest.Hostname = src.Hostname;
                }
            }
            else if (typeof(TSource) == typeof(CodeExecutionAuditEntity) && typeof(TDestination) == typeof(CodeExecutionAudit))
            {
                var src = source as CodeExecutionAuditEntity;
                var dest = destination as CodeExecutionAudit;
                if (src != null && dest != null)
                {
                    dest.Id = src.Id;
                    dest.StartedOnUTC = src.StartedOnUTC;
                    dest.CompletedOnUTC = src.CompletedOnUTC;
                    dest.CodeToRun = src.CodeToRun;
                    dest.UserAgentId = src.UserAgentId;
                    dest.EnvironmentName = src.EnvironmentName;
                    dest.IsExecutionError = src.IsExecutionError;
                    dest.ExecutionResult = src.ExecutionResult;
                    dest.Hostname = src.Hostname;
                }
            }
            else
            {
                throw new NotSupportedException($"Mapping from {typeof(TSource).Name} to {typeof(TDestination).Name} is not supported");
            }

            return destination;
        }

        public IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source) where TDestination : new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Select(item => Map<TSource, TDestination>(item)).ToList();
        }
    }
}
