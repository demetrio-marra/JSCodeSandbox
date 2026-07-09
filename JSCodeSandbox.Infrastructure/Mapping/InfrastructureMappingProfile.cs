namespace JSCodeSandbox.Infrastructure.Mapping
{
    public static class InfrastructureMappingProfile
    {
        public static IMapper CreateMapper()
        {
            return new MappingService();
        }
    }
}

