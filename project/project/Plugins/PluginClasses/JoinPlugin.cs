using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using project.DatabaseHealthChecker.Abstraction;
using project.Models.pluginoutput;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;

namespace project.Plugins.PluginClasses
{
    public class JoinPlugin : IPlugin
    {
        public string PluginName => "JoinPlugin";
        private readonly ILogger<JoinPlugin> _logger;
        private readonly IDatabaseHealthChecker _dbChecker;

        public JoinPlugin(
            ILogger<JoinPlugin> logger,
            IDatabaseHealthChecker dbChecker,
            IConfiguration configuration)
        {
            _logger = logger;
            _dbChecker = dbChecker;
        }

        public async Task<PluginOutput> Makequery(JsonElement commandelement, List<PluginOutput> pastOutputs = null)
        {
            _logger.LogInformation("Executing JoinPlugin...");

            if (pastOutputs == null || pastOutputs.Count < 2)
            {
                _logger.LogError("JoinPlugin requires at least two previous plugin outputs to operate.");
                throw new ArgumentException("JoinPlugin requires at least two previous plugin outputs.");
            }

            string jsoncommanddata = commandelement.GetRawText();
            var command = JsonSerializer.Deserialize<JoinModel>(jsoncommanddata);
            
            if (command == null || string.IsNullOrWhiteSpace(command.Type) || 
                string.IsNullOrWhiteSpace(command.OnFirst) || string.IsNullOrWhiteSpace(command.OnSecond))
            {
                _logger.LogError("Invalid command data for JoinPlugin. 'type', 'onfirst', and 'onsecond' are required.");
                throw new ArgumentException("Invalid command data: 'type', 'onfirst', and 'onsecond' must be provided.");
            }

            var firstQueryData = pastOutputs[0];
            var secondQueryData = pastOutputs[1];
            
            var firstQuery = $"({firstQueryData.Query})";
            var secondQuery = $"({secondQueryData.Query})";
            var firstConnection = firstQueryData.ConnectionString;
            var secondConnection = secondQueryData.ConnectionString;

            if (string.IsNullOrEmpty(firstQuery) || string.IsNullOrEmpty(secondQuery))
            {
                _logger.LogError("One or both of the past queries are null or empty.");
                throw new ArgumentNullException("Past queries cannot be null or empty for JoinPlugin.");
            }

            if (firstConnection == secondConnection)
            {
                string onClause = $"T1.\"{command.OnFirst}\" = T2.\"{command.OnSecond}\"";

                string finalQuery = $"SELECT * FROM {firstQuery} AS T1 {command.Type.ToUpper()} JOIN {secondQuery} AS T2 ON {onClause}";

                _logger.LogInformation("Successfully generated JOIN query for connection '{connection}'.", firstConnection);
                
                return new PluginOutput(finalQuery, firstConnection);
            }
            else
            {
                _logger.LogWarning("Cross-database join attempted between '{conn1}' and '{conn2}'. This is not supported.", firstConnection, secondConnection);
                throw new NotImplementedException("Cross-database joins are not supported by this plugin.");
            }
        }
    }
}
