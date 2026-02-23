using AutoMapper;
using JSCodeSandbox.Application.Models;
using JSCodeSandbox.Infrastructure.Entities;

namespace JSCodeSandbox.Infrastructure.Mapping
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            CreateMap<CodeExecutionEnvironment, CodeExecutionEnvironmentEntity>()
                .ReverseMap();
        }
    }
}
