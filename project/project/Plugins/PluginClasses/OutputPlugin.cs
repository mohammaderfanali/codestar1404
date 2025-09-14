using System.Text.Json;
using Microsoft.Extensions.Logging;
using project.DataBase.CreateTableFromQuery.Abstraction;
using project.Models.pluginoutput;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;
using project.TransferTablefromQuery.Abstraction;

namespace project.Plugins.PluginClasses
{
    public class OutputPlugin : IPlugin
    {
        public string PluginName => "OutputPlugin";
        private readonly ILogger<OutputPlugin> _logger;
        private readonly string _storeConnectionString;
        private readonly ITableCreator _tableCreator;
        private readonly IDataInserter _dataInserter;


        public OutputPlugin(ILogger<OutputPlugin> logger, ITableCreator tableCreator, IDataInserter dataInserter)
        {
            _logger = logger;
            _storeConnectionString = path.uploadconnection;
            _tableCreator = tableCreator;
            _dataInserter = dataInserter;
        }

        public async Task<PluginOutput> Makequery(JsonElement commandelement, CancellationToken cancellationToken,
            List<PluginOutput> parentOutputs)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Executing OutputPlugin...");

            if (parentOutputs == null || !parentOutputs.Any())
            {
                _logger.LogError("OutputPlugin requires at least one parent query result.");
                throw new ArgumentException("OutputPlugin requires a parent query result.");
            }

            var command = JsonSerializer.Deserialize<OutputPluginModel>(commandelement.GetRawText());
            var tableName = command?.TableName;

            if (string.IsNullOrWhiteSpace(tableName))
            {
                _logger.LogError("TableName must be provided in the JSON command for OutputPlugin.");
                throw new InvalidOperationException("TableName is not configured for OutputPlugin.");
            }

            var finalQueryToStore = parentOutputs.First().Query;
            var finalconnectionstring = parentOutputs.First().ConnectionString;

            try
            {
                await _tableCreator.CreateTableFromQueryAsync(
                    finalconnectionstring, finalQueryToStore, _storeConnectionString, tableName, cancellationToken);
                await _dataInserter.TransferDataAsync(
                    finalconnectionstring, finalQueryToStore, _storeConnectionString, tableName, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Execution of OutputPlugin for table '{TableName}' was canceled.", tableName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing OutputPlugin for table '{TableName}'.",
                    tableName);
                throw;
            }

            return parentOutputs.First();
        }
    }
}