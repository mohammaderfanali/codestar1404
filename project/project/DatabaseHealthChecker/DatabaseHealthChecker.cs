using System;
using System.Threading.Tasks;
using Npgsql;
using project.DatabaseHealthChecker.Abstraction;

namespace project.DatabaseHealthChecker
{
    
    
    public class DatabaseHealthChecker : IDatabaseHealthChecker
    {
        public async Task<bool> IsConnectionValidAsync(string connectionString)
        {
            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection check failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> TableHasDataAsync(string connectionString, string tableName)
        {
            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                var query = $"SELECT 1 FROM {tableName} LIMIT 1";

                await using var command = new NpgsqlCommand(query, connection);
                
                var result = await command.ExecuteScalarAsync();

                return result != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Table check failed for '{tableName}': {ex.Message}");
                return false;
            }
        }
    }
}