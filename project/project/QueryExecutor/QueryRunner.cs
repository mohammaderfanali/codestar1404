using System.Data;
using Npgsql;

namespace project.QueryExecutor
{
    public class QueryExecutor
    {
        public async Task<DataTable> ExecuteQueryAsync(KeyValuePair<string, string> queryPair)
        {
            using var connection = new NpgsqlConnection(queryPair.Value);
            using var command = new NpgsqlCommand(queryPair.Key, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var result = new DataTable();
            result.Load(reader);
            return result;
        }
    }
}
