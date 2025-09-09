using System.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using project.QueryExecutor.Abstraction;

namespace project.QueryExecutor
{
    public class QueryExecutor : IQueryExecutor
    {
        private readonly ILogger<QueryExecutor> _logger;

        public QueryExecutor(ILogger<QueryExecutor> logger)
        {
            _logger = logger;
        }

        public async Task<DataTable> ExecuteQueryAsync(string query, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogWarning("ExecuteQueryAsync called with null or empty query/connection string.");
                throw new ArgumentException("Query and connection string must not be null or empty.");
            }

            _logger.LogInformation("Executing query: {Query}", query);
            var result = new DataTable();

            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                
                await using var command = new NpgsqlCommand(query+=";", connection);
                await using var reader = await command.ExecuteReaderAsync();

                result.Load(reader);
                _logger.LogInformation("Query executed successfully, returning {RowCount} rows.", result.Rows.Count);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "A PostgreSQL error occurred while executing the query: {Query}", query);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while executing the query: {Query}", query);
                throw;
            }

            return result;
        }
    }
}