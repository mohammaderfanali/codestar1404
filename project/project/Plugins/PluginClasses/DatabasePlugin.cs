using System.Text.Json;
using Microsoft.Extensions.Logging;
using project.DatabaseHealthChecker.Abstraction;
using project.Models.pluginoutput;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;

namespace project.Plugins.PluginClasses
{
    public class DatabasePlugin : IPlugin
    {
        public string PluginName => "DatabasePlugin";

        private readonly ILogger<DatabasePlugin> _logger;
        private readonly IDatabaseHealthChecker _dbChecker;

        public DatabasePlugin(
            ILogger<DatabasePlugin> logger,
            IDatabaseHealthChecker dbChecker)
        {
            _logger = logger;
            _dbChecker = dbChecker;
        }

        public async Task<PluginOutput> Makequery(JsonElement commandelement, CancellationToken cancellationToken,
            List<PluginOutput> pastOutputs = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (pastOutputs != null && pastOutputs.Any())
            {
                _logger.LogWarning("DatabasePlugin does not support past outputs from other plugins.");
                throw new ArgumentException("DatabasePlugin requires an empty output history to run.");
            }

            string jsoncommanddata = commandelement.GetRawText();

            _logger.LogInformation("Running DatabasePlugin...");
            var config = JsonSerializer.Deserialize<DatabaseRederModel>(jsoncommanddata);

            if (config == null)
            {
                _logger.LogError("Invalid command received for DatabasePlugin. Could not deserialize JSON.");
                throw new InvalidOperationException("Invalid command for DatabasePlugin.");
            }

            var connectionString = $"Host={config.Host};Port={config.Port};Username={config.Username};" +
                                   $"Password={config.Password};Database={config.Database};";

            string tableName = config.Tablename;
            string query = $"SELECT * FROM \"{tableName}\"";

            try
            {
                if (await _dbChecker.IsConnectionValidAsync(connectionString, cancellationToken))
                {
                    if (!await _dbChecker.TableHasDataAsync(connectionString, tableName, cancellationToken))
                    {
                        _logger.LogWarning("Table '{TableName}' is empty or does not exist.", tableName);
                        throw new InvalidOperationException("Table '" + tableName + "' is empty or does not exist.");
                        ;
                    }

                    _logger.LogInformation("DatabasePlugin executed successfully.");
                    return new PluginOutput(query, connectionString);
                }
                else
                {
                    _logger.LogError("DatabasePlugin failed: Connection is not valid.");
                    throw new InvalidOperationException("Database connection is not valid.");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Execution of DatabasePlugin was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing DatabasePlugin.");
                throw;
            }
        }
    }
}