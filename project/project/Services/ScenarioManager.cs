using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using project.Services.Abstraction;

namespace project.Services
{
    public class ScenarioManager : IScenarioManager
    {
        private readonly ILogger<ScenarioManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IScenarioStatusUpdater _statusUpdater;
        private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _runningScenarios = new();

        public ScenarioManager(ILogger<ScenarioManager> logger, IServiceProvider serviceProvider, IScenarioStatusUpdater statusUpdater)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _statusUpdater = statusUpdater;
        }

        public Guid StartScenario(DataFlow.Models.Graph dag)
        {
            var scenarioId = Guid.NewGuid();
            var cts = new CancellationTokenSource();

            if (!_runningScenarios.TryAdd(scenarioId, cts))
            {
                _logger.LogError("Failed to add scenario {ScenarioId} to the tracking dictionary.", scenarioId);
                throw new InvalidOperationException("Could not start scenario due to a conflict in scenario ID generation.");
            }

            _logger.LogInformation("Starting scenario with ID: {ScenarioId}", scenarioId);

            Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var scenarioService = scope.ServiceProvider.GetRequiredService<IScenarioService>();
                    await scenarioService.ExecuteScenarioAsync(scenarioId ,dag, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Scenario {ScenarioId} was canceled.", scenarioId);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Scenario {ScenarioId} failed with an unhandled exception.", scenarioId);
                }
                finally
                {
                    _runningScenarios.TryRemove(scenarioId, out _);
                    cts.Dispose();
                    _logger.LogInformation("Scenario {ScenarioId} has completed and been removed from tracking.", scenarioId);
                }
            });

            return scenarioId;
        }

        public bool CancelScenario(Guid scenarioId)
        {
            if (_runningScenarios.TryGetValue(scenarioId, out var cts))
            {
                if (!cts.IsCancellationRequested)
                {
                    _logger.LogWarning("Cancellation requested for scenario {ScenarioId}.", scenarioId);
                    _statusUpdater.MarkAsCancelingAsync(scenarioId);
                    cts.Cancel();
                    return true;
                }
                _logger.LogInformation("Cancellation was already requested for scenario {ScenarioId}.", scenarioId);
                return true;
            }

            _logger.LogWarning("Could not cancel scenario {ScenarioId}: not found or already completed.", scenarioId);
            return false;
        }
    }
}

