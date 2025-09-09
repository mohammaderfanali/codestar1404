using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using project.MyApplication.Abstraction;
using project.PluginManager.Abstraction;
using project.Plugins;
using project.Topological_sort.Models;

namespace project.MyApplication
{
    public class Application : IApplication
    {
        private readonly ILogger<Application> _logger;
        private readonly IPluginRunner _pluginRunner;
        private readonly IConfiguration _configuration;

        public Application(ILogger<Application> logger, IPluginRunner pluginRunner, IConfiguration configuration)
        {
            _logger = logger;
            _pluginRunner = pluginRunner;
            _configuration = configuration;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Application is starting.");

            string jsonPath = path.jsontest2; 

            if (string.IsNullOrEmpty(jsonPath) || !File.Exists(jsonPath))
            {
                _logger.LogError("Scenario file path is not configured or the file does not exist at: {Path}", jsonPath);
                return;
            }

            _logger.LogInformation("Found scenario file at: {Path}", jsonPath);
            var json = await File.ReadAllTextAsync(jsonPath);
            var dag = JsonSerializer.Deserialize<Graph>(json);

            if (dag != null)
            {
                await _pluginRunner.Runscenario(dag);
            }
            else
            {
                _logger.LogWarning("Failed to deserialize the scenario file.");
            }

            _logger.LogInformation("Application has finished its work.");
        }
    }
}