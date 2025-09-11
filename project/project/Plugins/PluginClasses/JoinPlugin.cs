using System.Text.Json;
using Microsoft.Extensions.Logging;
using project.DataBase.CreateTableFromQuery.Abstraction;
using project.Models.pluginoutput;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;
using project.TransferTablefromQuery.Abstraction;

namespace project.Plugins.PluginClasses
{
    public class JoinPlugin : IPlugin
    {
        public string PluginName => "JoinPlugin";
        private readonly ILogger<JoinPlugin> _logger;
        private readonly ITableCreator _tableCreator;
        private readonly IDataInserter _dataInserter;

        public JoinPlugin(
            ILogger<JoinPlugin> logger,
            ITableCreator tableCreator,
            IDataInserter dataInserter)
        {
            _logger = logger;
            _tableCreator = tableCreator;
            _dataInserter = dataInserter;
        }

        public async Task<PluginOutput> Makequery(JsonElement commandelement, CancellationToken cancellationToken,
            List<PluginOutput> pastOutputs = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Executing JoinPlugin...");

            try
            {
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
                    _logger.LogError(
                        "Invalid command data for JoinPlugin. 'type', 'onfirst', and 'onsecond' are required.");
                    throw new ArgumentException(
                        "Invalid command data: 'type', 'onfirst', and 'onsecond' must be provided.");
                }

                var firstQueryData = pastOutputs[0];
                var secondQueryData = pastOutputs[1];

                var firstQuery = $"({firstQueryData.Query})";
                var secondQuery = $"({secondQueryData.Query})";
                var firstConnection = firstQueryData.ConnectionString;
                var secondConnection = secondQueryData.ConnectionString;

                if (string.IsNullOrEmpty(firstQueryData.Query) || string.IsNullOrEmpty(secondQueryData.Query))
                {
                    _logger.LogError("One or both of the past queries are null or empty.");
                    throw new ArgumentNullException("Past queries cannot be null or empty for JoinPlugin.");
                }

                if (firstConnection == secondConnection)
                {
                    string onClause = $"T1.\"{command.OnFirst}\" = T2.\"{command.OnSecond}\"";
                    string finalQuery =
                        $"SELECT * FROM {firstQuery} AS T1 {command.Type.ToUpper()} JOIN {secondQuery} AS T2 ON {onClause}";
                    _logger.LogInformation("Successfully generated JOIN query for a single database connection.");
                    return new PluginOutput(finalQuery, firstConnection);
                }
                else
                {
                    _logger.LogInformation("Performing cross-database join. Staging data into the primary database.");
                    var destinationConnection = firstConnection;
                    var tempTableName = $"temp_join_{Guid.NewGuid():N}";

                    _logger.LogInformation("Creating temporary table '{TempTableName}' in destination.", tempTableName);

                    await _tableCreator.CreateTableFromQueryAsync(
                        sourceQuery: secondQueryData.Query,
                        sourceConnectionString: secondConnection,
                        destinationConnectionString: destinationConnection,
                        newTableName: tempTableName,
                        cancellationToken: cancellationToken);

                    await _dataInserter.TransferDataAsync(
                        sourceConnectionString: secondConnection,
                        sourceQuery: secondQueryData.Query,
                        destinationConnectionString: destinationConnection,
                        newTableName: tempTableName,
                        cancellationToken: cancellationToken);

                    _logger.LogInformation("Successfully staged data into '{TempTableName}'.", tempTableName);

                    var secondQueryInDestDb = $"(SELECT * FROM \"{tempTableName}\")";
                    string onClause = $"T1.\"{command.OnFirst}\" = T2.\"{command.OnSecond}\"";
                    string finalQuery =
                        $"SELECT * FROM {firstQuery} AS T1 {command.Type.ToUpper()} JOIN {secondQueryInDestDb} AS T2 ON {onClause}";
                    _logger.LogInformation("Successfully generated cross-database JOIN query.");

                    return new PluginOutput(finalQuery, destinationConnection);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Execution of JoinPlugin was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing JoinPlugin.");
                throw;
            }

        }
    }
}

