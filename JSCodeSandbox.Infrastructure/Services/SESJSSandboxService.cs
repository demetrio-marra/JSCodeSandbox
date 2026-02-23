using JSCodeSandbox.Application.Repositories;
using JSCodeSandbox.Exceptions;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace JSCodeSandbox.Infrastructure.Services
{
    public class SESJSSandboxService : ISandboxService
    {
        private readonly ILogger<SESJSSandboxService> _logger;
        private readonly SESJSSandboxServiceConfiguration _configuration;
        private readonly SemaphoreSlim _installSemaphore = new SemaphoreSlim(1, 1);

        public SESJSSandboxService(SESJSSandboxServiceConfiguration configuration,
            ILogger<SESJSSandboxService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task ProvisionAsync(string environmentName, string codeImplementation, string packageJson)
        {
            var sandboxPath = Path.Combine(_configuration.EnvironmentsBasePath, environmentName);

            if (IsEnvironmentInitialized(sandboxPath))
            {
                return;
            }

            try
            {
                await _installSemaphore.WaitAsync();

                if (IsEnvironmentInitialized(sandboxPath))
                {
                    return;
                }

                await ProvisionSandboxAsync(sandboxPath, codeImplementation, packageJson);
            }
            finally
            {
                _installSemaphore.Release();
            }
        }

        public async Task<string> RunCodeAsync(string environmentName, string userAgentId, string code, Dictionary<string, string> backends)
        {
            var sandboxRunnerPath = Path.Combine(_configuration.EnvironmentsBasePath, environmentName, "sandbox-runner.js");

            // save code to a temp js file
            var codeFilePath = System.IO.Path.GetTempFileName() + ".js";
            await System.IO.File.WriteAllTextAsync(codeFilePath, code);
            _logger.LogDebug("Saved code to temporary file: {codeFilePath}", codeFilePath);

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
                _logger.LogDebug("Executing command: {command}", startInfo.FileName + " " + startInfo.Arguments);

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


        private async Task ProvisionSandboxAsync(string sandboxPath, string codeImplementation, string packageJson)
        {
            Directory.CreateDirectory(sandboxPath);

            // create package.json file in the environment directory
            var packageJsonPath = Path.Combine(sandboxPath, "package.json");
            try
            {
                await File.WriteAllTextAsync(packageJsonPath, packageJson);
                _logger.LogInformation("Created package.json in environment directory: {packageJsonPath}", packageJsonPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create package.json in environment directory: {packageJsonPath}", packageJsonPath);
                throw new InfrastructureError(GetType().Name, $"Failed to create package.json in environment directory: {ex.Message}", ex);
            }

            // copy the sandbox-runner.js file to the environment directory
            var sourceSandboxRunnerPath = Path.Combine(AppContext.BaseDirectory, "JsFiles", "sandbox-runner.js");
            var destSandboxRunnerPath = Path.Combine(sandboxPath, "sandbox-runner.js");
            try
            {
                File.Copy(sourceSandboxRunnerPath, destSandboxRunnerPath, true);
                _logger.LogInformation("Copied sandbox-runner.js to environment directory: {destSandboxRunnerPath}", destSandboxRunnerPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to copy sandbox-runner.js to environment directory: {destSandboxRunnerPath}", destSandboxRunnerPath);
                throw new InfrastructureError(GetType().Name, $"Failed to copy sandbox-runner.js to environment directory: {ex.Message}", ex);
            }

            // create a new file called "tools-impl.js" in the environment directory and write the code implementation to it
            var toolsImplPath = Path.Combine(sandboxPath, "tools-impl.js");
            try
            {
                await File.WriteAllTextAsync(toolsImplPath, codeImplementation);
                _logger.LogInformation("Created tools-impl.js in environment directory: {toolsImplPath}", toolsImplPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create tools-impl.js in environment directory: {toolsImplPath}", toolsImplPath);
                throw new InfrastructureError(GetType().Name, $"Failed to create tools-impl.js in environment directory: {ex.Message}", ex);
            }


            var npmCommand = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "npm.cmd" : "npm";
            
            var installProcess = new System.Diagnostics.ProcessStartInfo
            {
                FileName = npmCommand,
                Arguments = "install",
                WorkingDirectory = sandboxPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var process = System.Diagnostics.Process.Start(installProcess))
            {
                if (process == null)
                {
                    throw new InfrastructureError(GetType().Name, "Failed to start npm install process");
                }

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                var output = await outputTask;
                var error = await errorTask;

                if (process.ExitCode != 0)
                {
                    throw new InfrastructureError(GetType().Name, $"Failed to install Node.js dependencies: {error}");
                }
            }
        }

        private bool IsEnvironmentInitialized(string sandboxPath)
        {
            if (!Directory.Exists(sandboxPath))
            {
                return false;
            }

            if (!File.Exists(Path.Combine(sandboxPath, "sandbox-runner.js")) || !File.Exists(Path.Combine(sandboxPath, "tools-impl.js")))
            {
                return false;
            }

            if (!Directory.Exists(Path.Combine(sandboxPath, "node_modules")))
            {
                return false;
            }

            return true;
        }


        public class SESJSSandboxServiceConfiguration
        {
            public string EnvironmentsBasePath { get; set; } = string.Empty;
            public string? NodeExtraCACertsPath { get; set; }
        }
    }
}
