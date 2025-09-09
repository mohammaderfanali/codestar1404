using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;
using project.DataBaseUpploader.Abstraction;

namespace project.DataBaseUpploader
{
    public class DataBaseUploader : IDataBaseUploader
    {
        private readonly ILogger<DataBaseUploader> _logger;

        public DataBaseUploader(ILogger<DataBaseUploader> logger)
        {
            _logger = logger;
        }

        private async Task CreateTableIfNotExistsAsync(NpgsqlConnection connection, string tableName, string[] headers)
        {
            _logger.LogInformation("Checking if table {TableName} exists...", tableName);
            var columnDefinitions = string.Join(", ", headers.Select(h => $"\"{h.Trim()}\" TEXT"));
            var sql = $"CREATE TABLE IF NOT EXISTS \"{tableName}\" ({columnDefinitions})";

            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
            _logger.LogInformation("Table {TableName} is ready.", tableName);
        }

        public async Task UploadDataAsync(string connectionString, string tableName, string[] headers, List<string[]> data)
        {
            if (data == null || !data.Any())
            {
                _logger.LogWarning("No data provided to upload for table {TableName}.", tableName);
                return;
            }

            _logger.LogInformation("Attempting to use connection string: \"{ConnectionString}\"", connectionString);

            await using var connection = new NpgsqlConnection(connectionString);

            try
            {
                await connection.OpenAsync();
                
                await CreateTableIfNotExistsAsync(connection, tableName, headers);
                
                var columnNames = string.Join(", ", headers.Select(h => $"\"{h.Trim()}\""));
                var valueParams = string.Join(", ", headers.Select((_, i) => $"@p{i}"));
                var sql = $"INSERT INTO \"{tableName}\" ({columnNames}) VALUES ({valueParams})";
                
                _logger.LogInformation("Starting upload of {RowCount} rows to table {TableName}.", data.Count, tableName);

                foreach (var row in data)
                {
                    await using var command = new NpgsqlCommand(sql, connection);
                    for (int i = 0; i < headers.Length; i++)
                    {
                        command.Parameters.AddWithValue($"p{i}", row.Length > i ? (object)row[i] : DBNull.Value);
                    }
                    await command.ExecuteNonQueryAsync();
                }

                _logger.LogInformation("Successfully uploaded data to {TableName}.", tableName);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "A PostgreSQL error occurred: {ErrorMessage}. Check connection details and database status.", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while connecting to or uploading data to table {TableName}.", tableName);
                throw;
            }
        }
    }
}

