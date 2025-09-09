using System.Data;
using System.Text;
using Microsoft.Extensions.Logging;
using Npgsql;
using project.CreateTableFromQuery.Abstraction;

namespace project.CreateTableFromQuery
{
    public class TableCreator : ITableCreator
    {
        private readonly ILogger<TableCreator> _logger;

        public TableCreator(ILogger<TableCreator> logger)
        {
            _logger = logger;
        }

        public async Task CreateTableFromQueryAsync(string sourceConnectionString, string sourceQuery, string destinationConnectionString, string newTableName)
        {
            _logger.LogInformation("Starting table creation process for '{NewTableName}'.", newTableName);

            DataTable schemaTable;
            try
            {
                await using var sourceConnection = new NpgsqlConnection(sourceConnectionString);
                await sourceConnection.OpenAsync();
                await using var command = new NpgsqlCommand(sourceQuery, sourceConnection);

                await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
                schemaTable = await reader.GetSchemaTableAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve schema from the source query.");
                throw;
            }

            if (schemaTable == null)
            {
                throw new InvalidOperationException($"Could not derive schema for table '{newTableName}'.");
            }
            
            var createTableSql = GenerateCreateTableSql(newTableName, schemaTable);

            try
            {
                await using var destConnection = new NpgsqlConnection(destinationConnectionString);
                await destConnection.OpenAsync();
                
                await using var dropCommand = new NpgsqlCommand($"DROP TABLE IF EXISTS \"{newTableName}\";", destConnection);
                await dropCommand.ExecuteNonQueryAsync();

                await using var createCommand = new NpgsqlCommand(createTableSql, destConnection);
                await createCommand.ExecuteNonQueryAsync();
                _logger.LogInformation("Successfully created table '{NewTableName}' in the destination database.", newTableName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create table '{NewTableName}' in the destination database.", newTableName);
                throw;
            }
        }

        private string GenerateCreateTableSql(string tableName, DataTable schemaTable)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"CREATE TABLE \"{tableName}\" (");

            var processedColumnNames = new HashSet<string>();
            var columnDefinitions = new List<string>();

            foreach (DataRow row in schemaTable.Rows)
            {
                var originalColumnName = (string)row["ColumnName"];
                var finalColumnName = originalColumnName;
                int suffix = 1;

                while (processedColumnNames.Contains(finalColumnName))
                {
                    finalColumnName = $"{originalColumnName}_{suffix++}";
                }

                processedColumnNames.Add(finalColumnName);
                var columnType = GetPostgresType((Type)row["DataType"]);
                columnDefinitions.Add($"    \"{finalColumnName}\" {columnType}");
            }

            builder.AppendLine(string.Join(",\n", columnDefinitions));
            builder.AppendLine(");");

            return builder.ToString();
        }

        private string GetPostgresType(Type dotnetType) => dotnetType.Name switch
        {
            "Int32" => "INTEGER",
            "Int64" => "BIGINT",
            "String" => "TEXT",
            "Decimal" => "NUMERIC",
            "Double" => "DOUBLE PRECISION",
            "Boolean" => "BOOLEAN",
            "DateTime" => "TIMESTAMP WITH TIME ZONE",
            _ => "TEXT" 
        };
    }
}

