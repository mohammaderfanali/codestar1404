using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using project.DatabaseHealthChecker.Abstraction;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;

namespace project.Plugins.PluginClasses;

public class DatabasePlugin : IPlugin
{
    public string PluginName => "DatabasePlugin";

    private readonly ILogger<DatabasePlugin> _logger;
    private readonly IDatabaseHealthChecker _dbChecker;
    private readonly string _connectionString;

    public DatabasePlugin(
        ILogger<DatabasePlugin> logger,
        IDatabaseHealthChecker dbChecker,
        IConfiguration configuration)
    {
        _logger = logger;
        _dbChecker = dbChecker;
        _connectionString = configuration.GetConnectionString("UploadConnection");
    }

    public async Task<KeyValuePair<string, string>> Makequery(JsonElement commandelement,
        List<KeyValuePair<string, string>> pastquery = null)
    {
        if (pastquery != null && pastquery.Count != 0)
        {
            _logger.LogWarning("DatabasePlugin does not support past query inputs.");
            throw new ArgumentException("DatabaseReader requires an empty query history to run.");
        }

        string jsoncommanddata = commandelement.GetRawText();
        _logger.LogInformation("Running DatabasePlugin...");

        var command = JsonSerializer.Deserialize<DatabaseRederModel>(jsoncommanddata);
        if (command == null)
        {
            _logger.LogError("Invalid command received for DatabasePlugin.");
            return new KeyValuePair<string, string>();
        }

        string tableName = command.Tablename;
        string query = $"SELECT * FROM [{tableName}];";

        try
        {
            if (await _dbChecker.IsConnectionValidAsync(_connectionString))
            {
                if (!await _dbChecker.TableHasDataAsync(_connectionString, tableName))
                {
                    _logger.LogWarning("Table '{TableName}' is empty or does not exist.", tableName);
                }
                _logger.LogInformation("DatabasePlugin executed successfully.");
                return new KeyValuePair<string, string>(query, _connectionString);
            }
            else
            {
                _logger.LogError("DatabasePlugin failed: Connection is not valid.");
                return new KeyValuePair<string, string>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing DatabasePlugin.");
            return new KeyValuePair<string, string>();
        }
    }
}
