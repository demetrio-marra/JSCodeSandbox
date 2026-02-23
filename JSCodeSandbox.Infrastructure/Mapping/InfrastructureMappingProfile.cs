using AutoMapper;
using JSCodeSandbox.Application.Models;
using JSCodeSandbox.Infrastructure.Entities;
using JSCodeSandbox.Models;

namespace JSCodeSandbox.Infrastructure.Mapping
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            CreateMap<CodeExecutionEnvironment, CodeExecutionEnvironmentEntity>()
                .ReverseMap();
            
            CreateMap<CodeExecutionAudit, CodeExecutionAuditEntity>()
                .ReverseMap();
        }
    }
}
