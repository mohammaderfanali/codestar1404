using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using project.DataFlow.Models;
using project.Graph.Models;
using project.Plugins.RunPlugin.Abstraction;
using project.Plugins.RunPlugin.Abstraction;
using project.Services.Abstraction;

namespace project.Services
{
    public class ScenarioService : IScenarioService
    {
        private readonly ILogger<ScenarioService> _logger;
        private readonly IDataFlowRunner _pluginRunner;

        public ScenarioService( ILogger<ScenarioService> logger, IDataFlowRunner pluginRunner)
        {
            _logger = logger;
            _pluginRunner = pluginRunner;
        }

        public async Task ExecuteScenarioAsync(Guid scenarioId, DataFlow.Models.Graph dag, CancellationToken cancellationToken)
        {
            if (dag == null)
            {
                _logger.LogWarning("Scenario graph is null. Nothing to execute.");
                return;
            }

            _logger.LogInformation("Executing scenario via API call.");
            
            await _pluginRunner.RunDataFlow(scenarioId,dag, cancellationToken);
        }
    }
}