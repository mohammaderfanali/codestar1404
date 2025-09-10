using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
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

        public async Task<PluginOutput> Makequery(JsonElement commandelement, List<PluginOutput> parentOutputs)
        {
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
                    finalconnectionstring, finalQueryToStore, _storeConnectionString, tableName);
                await _dataInserter.TransferDataAsync(
                        finalconnectionstring, finalQueryToStore, _storeConnectionString, tableName);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return parentOutputs.First();
        }
    }
}


