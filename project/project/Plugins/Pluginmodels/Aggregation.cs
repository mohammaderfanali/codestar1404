using System.Text.Json;
using Microsoft.Extensions.Logging;
using project.Models.pluginoutput;
using project.Plugins.Abstraction;
using project.Plugins.PluginClasses;

namespace project.Plugins.Pluginmodels
{
    public class AggregationPlugin : IPlugin
    {
        public string PluginName => "AggregationPlugin";
        private readonly ILogger<AggregationPlugin> _logger;
        private static readonly HashSet<string> ValidAggregationFunctions = new(StringComparer.OrdinalIgnoreCase)
        {
            "COUNT", "SUM", "AVG", "MIN", "MAX"
        };

        public AggregationPlugin(ILogger<AggregationPlugin> logger)
        {
            _logger = logger;
        }

        public async Task<PluginOutput> Makequery(JsonElement commandelement, List<PluginOutput> parentOutputs)
        {
            if (parentOutputs == null || parentOutputs.Count == 0)
            {
                _logger.LogError("AggregationPlugin requires a parent plugin output to operate on.");
                throw new ArgumentException("AggregationPlugin requires a parent plugin output.");
            }

            var command = JsonSerializer.Deserialize<AggregationModel>(commandelement.GetRawText());

            if (command == null || !IsValidCommand(command))
            {
                _logger.LogError("Invalid command data for AggregationPlugin.");
                throw new ArgumentException("Invalid command for AggregationPlugin. Please provide aggregationType, aggregationColumn, and at least one groupByColumn.");
            }

            var parentOutput = parentOutputs.First();
            var sourceQuery = parentOutput.Query;

            var groupByClause = string.Join(", ", command.GroupByColumns.Select(c => $"\"{c.Trim()}\""));
            var selectClause = $"{groupByClause}, {command.AggregationType.ToUpper()}(\"{command.AggregationColumn.Trim()}\") AS aggregated_value";
            
            var finalQuery = $"SELECT {selectClause} FROM ({sourceQuery}) AS data GROUP BY {groupByClause}";

            _logger.LogInformation("Successfully generated aggregation query.");
            return new PluginOutput(finalQuery, parentOutput.ConnectionString);
        }

        private bool IsValidCommand(AggregationModel command)
        {
            return !string.IsNullOrWhiteSpace(command.AggregationType) &&
                   ValidAggregationFunctions.Contains(command.AggregationType) &&
                   !string.IsNullOrWhiteSpace(command.AggregationColumn) &&
                   command.GroupByColumns != null && command.GroupByColumns.Any();
        }
    }
}
