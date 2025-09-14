using Microsoft.Extensions.Logging;
using project.DataFlow.GetParent;
using project.DataFlow.GetParent.Abstraction;
using project.DataFlow.Models.Identifier;
using project.DataFlow.Sort;
using project.DataFlow.Sort.Abstraction;
using project.Graph.Models;
using project.Models.pluginoutput;
using project.Plugins.Abstraction;
using project.Plugins.RunPlugin.Abstraction;

namespace project.Plugins.RunPlugin
{
    public class DataFlowRunner : IDataFlowRunner
    {
        private readonly IReadOnlyDictionary<string, IPlugin> _plugins;
        private readonly ILogger<DataFlowRunner> _logger;
        private readonly ITopologicalSorter _topologicalSorter;
        private readonly INodeParentProvider _nodeParentProvider;


        public DataFlowRunner(ILogger<DataFlowRunner> logger, IEnumerable<IPlugin> plugins,
            ITopologicalSorter topologicalSorter, INodeParentProvider nodeParentProvider)
        {
            _logger = logger;
            _plugins = plugins.ToDictionary(p => p.PluginName, p => p);
            _logger.LogInformation("Loaded {PluginCount} plugins.", _plugins.Count);
            _topologicalSorter = topologicalSorter;
            _nodeParentProvider = nodeParentProvider;
        }

        public async Task RunDataFlow(DataFlow.Models.Graph dag, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting scenario execution...");
            try
            {
                var sortedNodes = _topologicalSorter.Sort(dag);
                var parents = _nodeParentProvider.GetParents(dag);
                _logger.LogInformation("{NodeCount} nodes sorted for processing.", sortedNodes.Count);

                var results = new Dictionary<NodeId, PluginOutput>();

                foreach (var node in sortedNodes)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await ExecuteNodeAsync(node, parents, results, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during scenario execution.");
                throw;
            }

            _logger.LogInformation("Scenario execution finished.");
        }

        private async Task ExecuteNodeAsync(
            Node node,
            Dictionary<NodeId, List<NodeId>> parents,
            Dictionary<NodeId, PluginOutput> results,
            CancellationToken cancellationToken)
        {
            if (!_plugins.TryGetValue(node.Type, out var plugin))
            {
                _logger.LogWarning("No plugin found for node type '{NodeType}' with ID {NodeId}.", node.Type, node.Id);
                return;
            }

            try
            {
                var parentOutputs = new List<PluginOutput>();
                foreach (var parentId in parents[node.Id])
                {
                    if (results.TryGetValue(parentId, out var parentResult))
                    {
                        parentOutputs.Add(parentResult);
                    }
                    else
                    {
                        _logger.LogWarning("Parent result with ID {ParentId} not found for node {NodeId}.", parentId,
                            node.Id);
                    }
                }

                _logger.LogInformation("Executing plugin {PluginName} for node {NodeId}...", plugin.PluginName,
                    node.Id);

                var result = await plugin.Makequery(node.Data, cancellationToken, parentOutputs);
                results.Add(node.Id, result);

                _logger.LogInformation("Node {NodeId}: Result Query='{Query}'", node.Id, result.Query);
                _logger.LogInformation("Plugin {PluginName} for node {NodeId} executed successfully.",
                    plugin.PluginName, node.Id);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Scenario execution was canceled in PluginRunner.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Execution of plugin {PluginName} for node {NodeId} failed.", plugin.PluginName,
                    node.Id);
            }
        }
    }
}