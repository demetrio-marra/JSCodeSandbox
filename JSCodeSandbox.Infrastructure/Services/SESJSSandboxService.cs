using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Exceptions;
using Microsoft.Extensions.Logging;

namespace JSCodeSandbox.Infrastructure.Services
{
    public class SESJSSandboxService : ISandboxService
    {
        private readonly ILogger<SESJSSandboxService> _logger;
        private readonly SESJSSandboxServiceConfiguration _configuration;

        public SESJSSandboxService(SESJSSandboxServiceConfiguration configuration,
            ILogger<SESJSSandboxService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> RunCode(string environmentName, string userAgentId, string code, Dictionary<string, string> backends)
        {
            var sandboxRunnerPath = Path.Combine(_configuration.EnvironmentsBasePath, environmentName, "sandbox-runner.js");

            // save code to a temp js file
            var codeFilePath = System.IO.Path.GetTempFileName() + ".js";
            await System.IO.File.WriteAllTextAsync(codeFilePath, code);
            Console.WriteLine("Saved code to temporary file: " + codeFilePath);

            // change it to node sandbox-runner.js [agent-id] --file <codeFilePath>

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("node", sandboxRunnerPath + " " + userAgentId + " --file " + codeFilePath)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            if (_configuration.NodeExtraCACertsPath != null)
            {
                startInfo.EnvironmentVariables["NODE_EXTRA_CA_CERTS"] = Path.Combine(AppContext.BaseDirectory, _configuration.NodeExtraCACertsPath);
            }
          
            startInfo.EnvironmentVariables["JSSandboxBackends"] = System.Text.Json.JsonSerializer.Serialize(backends);

            using (var process = System.Diagnostics.Process.Start(startInfo))
            {
                if (process == null)
                {
                    throw new InfrastructureError(GetType().Name, "Failed to start Node.js process");
                }

                // log the complete command being run
                _logger.LogDebug("Executing command:\nnode " + sandboxRunnerPath + " " + userAgentId + " --file " + codeFilePath);

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                var output = await outputTask;
                var error = await errorTask;

                if (process.ExitCode != 0)
                {
                    throw new CodeExecutionException(process.ExitCode, error);
                }

                return output;
            }
        }


        public class SESJSSandboxServiceConfiguration
        {
            public string EnvironmentsBasePath { get; set; } = string.Empty;
            public string? NodeExtraCACertsPath { get; set; }
        }
    }
}
