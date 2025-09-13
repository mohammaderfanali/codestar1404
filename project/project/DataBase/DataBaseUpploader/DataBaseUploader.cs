using System.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using project.DataBase.DataBaseUpploader.Abstraction;

namespace project.DataBase.DataBaseUpploader
{
    public class DataBaseUploader : IDataBaseUploader
    {
        private readonly ILogger<DataBaseUploader> _logger;

        public DataBaseUploader(ILogger<DataBaseUploader> logger)
        {
            _logger = logger;
        }

        private async Task CreateTableIfNotExistsAsync(NpgsqlConnection connection, DataTable table,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking if table {TableName} exists...", table.TableName);
            var columnNames = table.Columns
                .Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();

            var columnDefinitions = string.Join(", ", columnNames.Select(h => $"\"{h.Trim()}\" TEXT"));
            var sql = $"CREATE TABLE IF NOT EXISTS \"{table.TableName}\" ({columnDefinitions})";

            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
            _logger.LogInformation("Table {TableName} is ready.", table.TableName);
        }

        public async Task UploadDataAsync(string connectionString, DataTable table, CancellationToken cancellationToken)
        {
            if (table == null || table.Rows.Count == 0)
            {
                _logger.LogWarning("No data provided to upload for table {TableName}.", table?.TableName ?? "Unknown");
                return;
            }

            var tableName = table.TableName;
            var columnNames = table.Columns
                .Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();

            _logger.LogInformation("Attempting to use connection string: \"{ConnectionString}\"", connectionString);

            await using var connection = new NpgsqlConnection(connectionString);

            try
            {
                await connection.OpenAsync(cancellationToken);

                await CreateTableIfNotExistsAsync(connection, table, cancellationToken);

                var quotedColumns = string.Join(", ", columnNames.Select(name => $"\"{name}\""));
                var valueParams = string.Join(", ", columnNames.Select((_, i) => $"@p{i}"));
                var sql = $"INSERT INTO \"{tableName}\" ({quotedColumns}) VALUES ({valueParams})";

                _logger.LogInformation("Starting upload of {RowCount} rows to table {TableName}.", table.Rows.Count,
                    tableName);

                foreach (DataRow row in table.Rows)
                {
                    await using var command = new NpgsqlCommand(sql, connection);
                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        var value = row[i] ?? DBNull.Value;
                        command.Parameters.AddWithValue($"p{i}", value);
                    }

                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                _logger.LogInformation("Successfully uploaded data to {TableName}.", tableName);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Data upload to table {TableName} was canceled.", tableName);
                throw;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex,
                    "A PostgreSQL error occurred: {ErrorMessage}. Check connection details and database status.",
                    ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An unexpected error occurred while connecting to or uploading data to table {TableName}.",
                    tableName);
                throw;
            }
        }
    }
}