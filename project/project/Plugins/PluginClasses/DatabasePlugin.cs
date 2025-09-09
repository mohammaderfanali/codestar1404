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

    public DatabasePlugin(
        ILogger<DatabasePlugin> logger,
        IDatabaseHealthChecker dbChecker,
        IConfiguration configuration)
    {
        _logger = logger;
        _dbChecker = dbChecker;
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
        var config = JsonSerializer.Deserialize<DatabaseRederModel>(jsoncommanddata);
        var connectionString= $"Host={config.Host};Port={config.Port};Username={config.Username};" +
               $"Password={config.Password};Database={config.Database};";
        
        if (config == null)
        {
            _logger.LogError("Invalid command received for DatabasePlugin.");
            return new KeyValuePair<string, string>();
        }

        string tableName = config.Tablename;
        string query = $"SELECT * FROM {tableName}";

        try
        {
            if (await _dbChecker.IsConnectionValidAsync(connectionString))
            {
                if (!await _dbChecker.TableHasDataAsync(connectionString, tableName))
                {
                    _logger.LogWarning("Table '{TableName}' is empty or does not exist.", tableName);
                }
                _logger.LogInformation("DatabasePlugin executed successfully.");
                return new KeyValuePair<string, string>(query, connectionString);
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
