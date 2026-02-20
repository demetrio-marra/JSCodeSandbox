
using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Application.Services;
using JSCodeSandbox.Infrastructure.Repositories;
using JSCodeSandbox.Services;
using JSCodeSandbox.WebAPI.Filters;
using Microsoft.OpenApi.Models;

namespace JSCodeSandbox.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationExceptionFilter>();
            });

            // Register MongoDB configuration
            var mongoConfig = builder.Configuration
                .GetSection("CodeExecutionEnvironmentsMongoRepository")
                .Get<CodeExecutionEnvironmentsMongoRepository.Configuration>();
            builder.Services.AddSingleton(mongoConfig ?? throw new InvalidOperationException("MongoDB configuration is missing"));

            // Register repositories and services
            builder.Services.AddSingleton<ICodeExecutionEnvironmentsRepository, CodeExecutionEnvironmentsMongoRepository>();
            builder.Services.AddScoped<ICodeExecutionEnvironmentsProvisioningService, CodeExecutionEnvironmentsProvisioningService>();
            builder.Services.AddScoped<ICodeExecutionService, CodeExecutionService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JSCodeSandbox API",
                    Version = "v1",
                    Description = "API for managing JavaScript code execution environments and executing code in sandboxed containers",
                    Contact = new OpenApiContact
                    {
                        Name = "JSCodeSandbox"
                    }
                });

                // Enable XML comments if available
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "JSCodeSandbox API v1");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "JSCodeSandbox API";
                options.DisplayRequestDuration();
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
