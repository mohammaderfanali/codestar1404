using System.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using project.DataBase.CreateTableQuery;
using project.DataBase.DataTableUpploader.Abstraction;
using project.Helpers.ColumnNameMaker.Abstraction;

namespace project.DataBase.DataBaseUpploader
{
    public class DataTableUplouder : IDataTableUplouder
    {
        private readonly ILogger<DataTableUplouder> _logger;
        private readonly IQueryTableGenerator _queryTableGenerator;
        private readonly IColumnNameResolver _columnNameResolver;

        public DataTableUplouder(
            ILogger<DataTableUplouder> logger,
            IQueryTableGenerator queryTableGenerator,
            IColumnNameResolver columnNameResolver)
        {
            _logger = logger;
            _queryTableGenerator = queryTableGenerator;
            _columnNameResolver = columnNameResolver;
        }

        public async Task UploadDataAsync(string connectionString, DataTable table, CancellationToken cancellationToken)
        {
            if (table == null || table.Rows.Count == 0)
            {
                _logger.LogWarning("No data provided to upload for table {TableName}.", table?.TableName ?? "Unknown");
                return;
            }

            var tableName = table.TableName;
            var columnNames = _columnNameResolver.ResolveUniqueNames(table.Columns);

            _logger.LogInformation("Attempting to use connection string: \"{ConnectionString}\"", connectionString);

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            try
            {
                var createQuery = _queryTableGenerator.GenerateCreateTableSql(tableName, table.Columns);
                await using (var createCommand = new NpgsqlCommand(createQuery, connection, transaction))
                {
                    await createCommand.ExecuteNonQueryAsync(cancellationToken);
                }
                    var quotedColumns = string.Join(", ", columnNames.Select(name => $"\"{name}\""));
                var valueParams = string.Join(", ", columnNames.Select((_, i) => $"@p{i}"));
                var insertSql = $"INSERT INTO \"{tableName}\" ({quotedColumns}) VALUES ({valueParams})";

                _logger.LogInformation("Starting upload of {RowCount} rows to table {TableName}.", table.Rows.Count,
                    tableName);

                foreach (DataRow row in table.Rows)
                {
                    await using var command = new NpgsqlCommand(insertSql, connection, transaction);
                    command.CommandTimeout = 0; // infinite

                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        var value = row[i] ?? DBNull.Value;
                        command.Parameters.AddWithValue($"p{i}", value);
                    }

                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Successfully uploaded data to {TableName}.", tableName);
            }
            catch (OperationCanceledException)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogWarning("Data upload to table {TableName} was canceled.", tableName);
                throw;
            }
            catch (NpgsqlException ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "PostgreSQL error: {ErrorMessage}.", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Unexpected error during upload to table {TableName}.", tableName);
                throw;
            }
        }
    }
}