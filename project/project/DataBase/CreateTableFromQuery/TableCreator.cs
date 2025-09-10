using System.Data;
using System.Text;
using Microsoft.Extensions.Logging;
using project.DataBase.CreateTableFromQuery.Abstraction;
using project.DataBase.QueryExecutor.Abstraction;

namespace project.DataBase.CreateTableFromQuery
{
    public class TableCreator : ITableCreator
    {
        private readonly ILogger<TableCreator> _logger;
        private readonly ISelectQueryExecutor _queryExecutor;

        public TableCreator(ILogger<TableCreator> logger, ISelectQueryExecutor queryExecutor)
        {
            _logger = logger;
            _queryExecutor = queryExecutor;
        }

        public async Task CreateTableFromQueryAsync(string sourceConnectionString, string sourceQuery, string destinationConnectionString, string newTableName)
        {
            _logger.LogInformation("Starting table creation process for '{NewTableName}'.", newTableName);

            var dataTable = await _queryExecutor.ExecuteQueryAsync(sourceQuery, sourceConnectionString);

            if (dataTable == null || dataTable.Columns.Count == 0)
            {
                throw new InvalidOperationException($"Could not derive schema for table '{newTableName}'. The query returned no columns.");
            }
            
            var createTableSql = GenerateCreateTableSql(newTableName, dataTable.Columns);

            try
            {
                var dropSql = $"DROP TABLE IF EXISTS \"{newTableName}\";";

                await _queryExecutor.ExecuteQueryAsync(dropSql, destinationConnectionString);

                await _queryExecutor.ExecuteQueryAsync(createTableSql, destinationConnectionString);
                
                _logger.LogInformation("Successfully created table '{NewTableName}' in the destination database.", newTableName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create table '{NewTableName}' in the destination database.", newTableName);
                throw;
            }
        }

        private string GenerateCreateTableSql(string tableName, DataColumnCollection columns)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"CREATE TABLE \"{tableName}\" (");

            var processedColumnNames = new HashSet<string>();
            var columnDefinitions = new List<string>();

            foreach (DataColumn column in columns)
            {
                var originalColumnName = column.ColumnName;
                var finalColumnName = originalColumnName;
                int suffix = 1;

                while (processedColumnNames.Contains(finalColumnName))
                {
                    finalColumnName = $"{originalColumnName}_{suffix++}";
                }

                processedColumnNames.Add(finalColumnName);
                var columnType = GetPostgresType(column.DataType);
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

