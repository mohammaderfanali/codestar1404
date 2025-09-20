using Microsoft.Extensions.Logging;
using Npgsql;
using project.DataBase.DatabaseHealthChecker.Abstraction;

namespace project.DataBase.DatabaseHealthChecker
{
    public class DatabaseHealthChecker : IDatabaseHealthChecker
    {
        private readonly ILogger<DatabaseHealthChecker> _logger;

        public DatabaseHealthChecker(ILogger<DatabaseHealthChecker> logger)
        {
            _logger = logger;
            
        }
        public async Task<bool> IsConnectionValidAsync(string connectionString,CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);
                return true;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Connection check was canceled.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection check failed.");
                return false;
            }
        }

        public async Task<bool> TableHasDataAsync(string connectionString, string tableName,CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);

                var query = $"SELECT 1 FROM {tableName} LIMIT 1";

                await using var command = new NpgsqlCommand(query, connection);

                var result = await command.ExecuteScalarAsync(cancellationToken);

                return result != null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Table check for '{TableName}' was canceled.", tableName);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Table check failed for '{TableName}'.", tableName);
                return false;
            }
        }
    }
}