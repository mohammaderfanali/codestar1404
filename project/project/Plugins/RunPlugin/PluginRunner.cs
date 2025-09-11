using Microsoft.Extensions.Logging;
using project.DataFlow;
using project.Graph;
using project.Models.pluginoutput;
using project.Plugins.Abstraction;
using project.Plugins.RunPlugin.Abstraction;

namespace project.Plugins.RunPlugin
{
    public class PluginRunner : IPluginRunner
    {
        private readonly IReadOnlyDictionary<string, IPlugin> _plugins;
        private readonly ILogger<PluginRunner> _logger;

        public PluginRunner(ILogger<PluginRunner> logger, IEnumerable<IPlugin> plugins)
        {
            _logger = logger;
            _plugins = plugins.ToDictionary(p => p.PluginName, p => p);
            _logger.LogInformation("Loaded {PluginCount} plugins.", _plugins.Count);
        }

        public async Task Runscenario(DataFlow.Models.Graph dag,CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting scenario execution...");
            try
            {
                var sorter = new TopologicalSorter();
                var parrNodes = new NodeParentProvider();

                var sortedNodes = sorter.Sort(dag);
                var parrents = parrNodes.GetParents(dag);
                _logger.LogInformation("{NodeCount} nodes sorted for processing.", sortedNodes.Count);

                var results = new Dictionary<int, PluginOutput>();
                
                foreach (var node in sortedNodes)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (_plugins.TryGetValue(node.Type, out var foundPlugin))
                    {
                        try
                        {
                            var parentOutputs = new List<PluginOutput>();
                            foreach (var parentId in parrents[node.Id])
                            {
                                if (results.TryGetValue(parentId, out var parentResult))
                                {
                                    parentOutputs.Add(parentResult);
                                }
                                else
                                {
                                    _logger.LogWarning("Parent result with ID {ParentId} not found for node {NodeId}.",
                                        parentId, node.Id);
                                }
                            }

                            _logger.LogInformation("Executing plugin {PluginName} for node {NodeId}...",
                                foundPlugin.PluginName, node.Id);

                            var result = await foundPlugin.Makequery(node.Data,cancellationToken, parentOutputs);
                            results.Add(node.Id, result);

                            _logger.LogInformation("Node {NodeId}: Result Query='{Query}'", node.Id, result.Query);

                            _logger.LogInformation("Plugin {PluginName} for node {NodeId} executed successfully.",
                                foundPlugin.PluginName, node.Id);
                        }
                        catch (OperationCanceledException)
                        {
                            _logger.LogWarning("Scenario execution was canceled in PluginRunner.");
                            throw;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Execution of plugin {PluginName} for node {NodeId} failed.",
                                foundPlugin.PluginName, node.Id);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No plugin found for node type '{NodeType}' with ID {NodeId}.", node.Type,
                            node.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during scenario execution.");
            }

            _logger.LogInformation("Scenario execution finished.");
        }
    }
}