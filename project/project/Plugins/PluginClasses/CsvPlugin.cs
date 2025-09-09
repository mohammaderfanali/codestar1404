using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using project.csvReader.Abstraction;
using project.DatabaseHealthChecker.Abstraction;
using project.DataBaseUpploader.Abstraction;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;

namespace project.Plugins.PluginClasses;

public class CsvPlugin : IPlugin
{
    public string PluginName => "CsvReaderPlugin";


    private readonly ILogger<CsvPlugin> _logger;
    private readonly ICsvReader _csvReader;
    private readonly IDataBaseUploader _dataBaseUploader;
    private readonly IDatabaseHealthChecker _dbChecker;
    private readonly string _connectionString;


    public CsvPlugin(
        ILogger<CsvPlugin> logger,
        ICsvReader csvReader,
        IDataBaseUploader dataBaseUploader,
        IDatabaseHealthChecker dbChecker,
        IConfiguration configuration) 
    {
        _logger = logger;
        _csvReader = csvReader;
        _dataBaseUploader = dataBaseUploader;
        _dbChecker = dbChecker;
        _connectionString = path.uploadconnection;
    }

    public async Task<KeyValuePair<string, string>> Makequery(JsonElement commandelement,
        List<KeyValuePair<string, string>> pastquery)
    {
        if (pastquery.Count != 0)
        {
            _logger.LogWarning("CsvPlugin does not support past query inputs.");
            throw new ArgumentException("CsvPlugin requires an empty query history to run.");
        }

        string jsoncommanddata = commandelement.GetRawText();
        var command = JsonSerializer.Deserialize<CsvReaderModel>(jsoncommanddata);

        if (command == null)
        {
            _logger.LogError("Invalid CsvPlugin command.");
            return new KeyValuePair<string, string>();
        }

        _logger.LogInformation("Executing CsvPlugin for file: {FilePath}", command.Filepath);

        try
        {
            var tablename = _csvReader.GetFileName(command.Filepath);
            var content = _csvReader.ReadCsvFile(command.Filepath);
            var columnHeaders = _csvReader.GetColumnHeaders(command.Filepath);

            await _dataBaseUploader.UploadDataAsync(_connectionString, tablename+"_copy", columnHeaders, content);

            var isConnected = await _dbChecker.IsConnectionValidAsync(_connectionString);
            var hasData = await _dbChecker.TableHasDataAsync(_connectionString, tablename);

            if (!isConnected || !hasData)
            {
                _logger.LogWarning("Data upload may not have been successful. IsConnected: {IsConnected}, HasData: {HasData}", isConnected, hasData);
            }

            string query = $"SELECT * FROM {tablename}";
            _logger.LogInformation("CsvPlugin executed successfully.");

            return new KeyValuePair<string, string>(query, _connectionString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing CsvPlugin.");
            return new KeyValuePair<string, string>();
        }
    }
}

