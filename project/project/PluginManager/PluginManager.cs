using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using project.PluginManager.Abstraction;
using project.Plugins.Abstraction;
using project.Topological_sort;
using project.Topological_sort.GetParent;
using project.Topological_sort.Models;

namespace project.PluginManager
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

        public async Task Runscenario(Graph dag)
        {
            _logger.LogInformation("Starting scenario execution...");
            try
            {
                var sorter = new TopologicalSorter();
                var parrNodes = new NodeParentProvider();

                var sortedNodes = sorter.Sort(dag);
                var parrents = parrNodes.GetParents(dag);
                _logger.LogInformation("{NodeCount} nodes sorted for processing.", sortedNodes.Count);

                var results = new Dictionary<int, KeyValuePair<string, string>>();

                foreach (var node in sortedNodes)
                {
                    if (_plugins.TryGetValue(node.Type, out var foundPlugin))
                    {
                        try
                        {
                            var parentInput = new List<KeyValuePair<string, string>>();
                            foreach (var parentId in parrents[node.Id])
                            {
                                if (results.TryGetValue(parentId, out var parentResult))
                                {
                                    parentInput.Add(parentResult);
                                }
                                else
                                {
                                    _logger.LogWarning("Parent result with ID {ParentId} not found for node {NodeId}.", parentId, node.Id);
                                }
                            }

                            _logger.LogInformation("Executing plugin {PluginName} for node {NodeId}...",
                                foundPlugin.PluginName, node.Id);
                            
                            var result = await foundPlugin.Makequery(node.Data, parentInput);
                            results.Add(node.Id, result);
                            
                            _logger.LogInformation("Node {NodeId}: Result Key='{ResultKey}', Result Value='{ResultValue}'", node.Id, result.Key, result.Value);

                            _logger.LogInformation("Plugin {PluginName} for node {NodeId} executed successfully.",
                                foundPlugin.PluginName, node.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Execution of plugin {PluginName} for node {NodeId} failed.",
                                foundPlugin.PluginName, node.Id);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No plugin found for node type '{NodeType}' with ID {NodeId}.", node.Type, node.Id);
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
