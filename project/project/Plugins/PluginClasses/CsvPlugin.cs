using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using project.DataBaseUpploader.Abstraction;
using project.Models.pluginoutput;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;
using project.ReadCsv.Abstraction;

namespace project.Plugins.PluginClasses
{
    public class CsvPlugin : IPlugin
    {
        public string PluginName => "CsvReaderPlugin";

        private readonly ILogger<CsvPlugin> _logger;
        private readonly ICsvReader _csvReader;
        private readonly IDataBaseUploader _dataBaseUploader;
        private readonly string _connectionString;

        public CsvPlugin(
            ILogger<CsvPlugin> logger,
            ICsvReader csvReader,
            IDataBaseUploader dataBaseUploader,
            IConfiguration configuration) 
        {
            _logger = logger;
            _csvReader = csvReader;
            _dataBaseUploader = dataBaseUploader;
            _connectionString = path.uploadconnection;

            if (string.IsNullOrEmpty(_connectionString))
            {
                _logger.LogError("DefaultConnection string is not configured in resources.");
                throw new InvalidOperationException("DefaultConnection string is not configured.");
            }
        }

        public async Task<PluginOutput> Makequery(JsonElement commandelement,CancellationToken cancellationToken, List<PluginOutput> pastOutputs = null)
        {
            if (pastOutputs != null && pastOutputs.Any())
            {
                _logger.LogWarning("CsvPlugin does not support past outputs from other plugins.");
                throw new ArgumentException("CsvPlugin requires an empty output history to run.");
            }

            string jsoncommanddata = commandelement.GetRawText();
            var command = JsonSerializer.Deserialize<CsvReaderModel>(jsoncommanddata);

            if (command == null || string.IsNullOrWhiteSpace(command.Filepath))
            {
                _logger.LogError("Invalid CsvPlugin command. 'FilePath' is required.");
                throw new InvalidOperationException("Invalid command for CsvPlugin: FilePath is missing.");
            }
            
            _logger.LogInformation("Executing CsvPlugin for file: {FilePath}", command.Filepath);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var tableName = Path.GetFileNameWithoutExtension(command.Filepath);
                var content = _csvReader.ReadCsvFile(command.Filepath);
                var columnHeaders = _csvReader.GetColumnHeaders(command.Filepath);

                await _dataBaseUploader.UploadDataAsync(_connectionString, tableName, columnHeaders, content,cancellationToken);

                string query = $"SELECT * FROM \"{tableName}\"";
                _logger.LogInformation("CsvPlugin executed successfully, returning query for table '{TableName}'.", tableName);

                return new PluginOutput(query, _connectionString);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Execution of CsvPlugin for file {FilePath} was canceled.", command.Filepath);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing CsvPlugin.");
                throw;
            }
        }
    }
}

