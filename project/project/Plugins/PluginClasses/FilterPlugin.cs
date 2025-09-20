using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using project.ConditionToStr.Abstraction;
using project.Models.pluginoutput;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;

namespace project.Plugins.PluginClasses
{
    public class FilterPlugin : IPlugin
    {
        public string PluginName => "FilterPlugin";
        private readonly ILogger<FilterPlugin> _logger;
        private readonly IConditionFormatter _conditionFormatter;

        public FilterPlugin(ILogger<FilterPlugin> logger, IConditionFormatter conditionFormatter)
        {
            _logger = logger;
            _conditionFormatter = conditionFormatter;
        }

        public async Task<PluginOutput> Makequery(JsonElement commandelement, CancellationToken cancellationToken,
            List<PluginOutput> parentOutputs = null!)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (parentOutputs == null || !parentOutputs.Any())
                {
                    throw new ArgumentException("FilterPlugin requires a parent plugin output to filter.");
                }


                var command = JsonSerializer.Deserialize<FilterPluginModel>(commandelement.GetRawText());

                if (command?.FilterGroups == null || !command.FilterGroups.Any())
                {
                    throw new ArgumentException(
                        "Invalid command for SimpleFilterPlugin. A non-empty 'filterGroups' array must be provided.");
                }

                var parentOutput = parentOutputs.First();
                var sourceQuery = parentOutput.Query;

                var andClauses = command.FilterGroups
                    .Where(group => group.Conditions.Any())
                    .Select(orGroup =>
                    {
                        var orConditions = orGroup.Conditions.Select(_conditionFormatter.Format);
                        return $"({string.Join(" OR ", orConditions)})";
                    })
                    .ToList();

                if (!andClauses.Any())
                {
                    _logger.LogWarning("No valid filter conditions were provided. Returning the original query.");
                    return parentOutput;
                }

                var whereClause = string.Join(" AND ", andClauses);
                var finalQuery = $"SELECT * FROM ({sourceQuery})  WHERE {whereClause}";

                _logger.LogInformation("Generated filter query: {FinalQuery}", finalQuery);

                await Task.CompletedTask;

                return new PluginOutput(finalQuery, parentOutput.ConnectionString);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Execution of SimpleFilterPlugin was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing SimpleFilterPlugin.");
                throw;
            }
        }
    }
}