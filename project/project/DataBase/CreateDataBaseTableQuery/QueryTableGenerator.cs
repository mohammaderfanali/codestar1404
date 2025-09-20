using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Extensions.Logging;
using project.Helpers.ColumnNameMaker.Abstraction;

namespace project.DataBase.CreateTableQuery
{
    public class QueryTableGenerator : IQueryTableGenerator
    {
        private readonly ILogger<QueryTableGenerator> _logger;
        private readonly IColumnNameResolver _columnNameResolver;

        public QueryTableGenerator(ILogger<QueryTableGenerator> logger, IColumnNameResolver columnNameResolver)
        {
            _logger = logger;
            _columnNameResolver = columnNameResolver;
        }

        public string GenerateCreateTableSql(string tableName, DataColumnCollection columns)
        {
            _logger.LogInformation("Generating CREATE TABLE SQL for table: {TableName}", tableName);

            var builder = new StringBuilder();
            builder.AppendLine($"DROP TABLE IF EXISTS \"{tableName}\";");
            builder.AppendLine($"CREATE TABLE \"{tableName}\" (");

            var finalColumnNames = _columnNameResolver.ResolveUniqueNames(columns);
            var columnDefinitions = new List<string>();

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                var finalName = finalColumnNames[i];
                var columnType = MapToPostgresType(column.DataType);

                _logger.LogDebug("Column: {OriginalName} mapped to {FinalName} with type {Type}",
                    column.ColumnName, finalName, columnType);

                columnDefinitions.Add($"    \"{finalName}\" {columnType}");
            }

            builder.AppendLine(string.Join(",\n", columnDefinitions));
            builder.AppendLine(");");

            var sql = builder.ToString();
            _logger.LogInformation("Generated SQL:\n{Sql}", sql);

            return sql;
        }

        private string MapToPostgresType(Type type) => type.Name switch
        {
            nameof(Int32) => "INTEGER",
            nameof(Int64) => "BIGINT",
            nameof(String) => "TEXT",
            nameof(Decimal) => "NUMERIC",
            nameof(Double) => "DOUBLE PRECISION",
            nameof(Boolean) => "BOOLEAN",
            nameof(DateTime) => "TIMESTAMP WITH TIME ZONE",
            _ => "TEXT"
        };
    }
}
