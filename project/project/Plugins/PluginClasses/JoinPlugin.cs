using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using project.DatabaseHealthChecker.Abstraction;
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

        public async Task<KeyValuePair<string, string>> Makequery(JsonElement commandelement,
            List<KeyValuePair<string, string>> pastquery = null)
        {
            _logger.LogInformation("Executing JoinPlugin...");

            if (pastquery == null || pastquery.Count < 2)
            {
                _logger.LogError("JoinPlugin requires at least two previous queries to operate.");
                throw new ArgumentException("JoinPlugin requires at least two previous queries.");
            }

            string jsoncommanddata = commandelement.GetRawText();
            var command = JsonSerializer.Deserialize<JoinModel>(jsoncommanddata);
            
            if (command == null || string.IsNullOrWhiteSpace(command.Type) || 
                string.IsNullOrWhiteSpace(command.OnFirst) || string.IsNullOrWhiteSpace(command.OnSecond))
            {
                _logger.LogError("Invalid command data for JoinPlugin. 'type', 'onfirst', and 'onsecond' are required.");
                throw new ArgumentException("Invalid command data: 'type', 'onfirst', and 'onsecond' must be provided.");
            }

            var firstQueryData = pastquery[0];
            var secondQueryData = pastquery[1];
            
            var firstQuery = $"({firstQueryData.Key})";
            var secondQuery = $"({secondQueryData.Key})";
            var firstConnection = firstQueryData.Value;
            var secondConnection = secondQueryData.Value;

            if (string.IsNullOrEmpty(firstQuery) || string.IsNullOrEmpty(secondQuery))
            {
                _logger.LogError("One or both of the past queries are null or empty.");
                throw new ArgumentNullException("Past queries cannot be null or empty for JoinPlugin.");
            }

            if (firstConnection == secondConnection)
            {
                string onClause = $"T1.{command.OnFirst} = T2.{command.OnSecond}";

                string finalQuery = $"SELECT * FROM {firstQuery} AS T1 {command.Type.ToUpper()} JOIN {secondQuery} AS T2 ON {onClause}";

                _logger.LogInformation("Successfully generated JOIN query for connection '{connection}'.", firstConnection);
                
                return new KeyValuePair<string, string>(finalQuery, firstConnection);
            }
            else
            {
                _logger.LogWarning("Cross-database join attempted between '{conn1}' and '{conn2}'. This is not supported.", firstConnection, secondConnection);
                throw new NotImplementedException("Cross-database joins are not supported by this plugin.");
            }
        }
    }
}