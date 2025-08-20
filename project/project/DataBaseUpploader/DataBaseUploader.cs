using Npgsql;
using System.Data;
using project.DataBaseUpploader.Abstraction;

namespace project.DataBaseUpploader;

public class DataBaseUploader : IDataBaseUploader
{
    public async Task UploadDataAsync(
        string connectionString, 
        string tableName, 
        List<string[]> data, 
        bool skipHeaderRow = true)
    {
        
        if (data.Count == 0)
        {
            Console.WriteLine("No data to upload.");
            return;
        }

        var columnNames = data[0]; 
        var dataToInsert = skipHeaderRow ? data.Skip(1) : data;
        
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        
        try
        {
            var columnDefinitions = string.Join(", ", columnNames.Select(name => $"\"{name}\" TEXT"));
            var createTableCommandText = $"CREATE TABLE IF NOT EXISTS {tableName} ({columnDefinitions});";
            
            await using (var cmd = new NpgsqlCommand(createTableCommandText, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            var copyCommand = $"COPY {tableName} ({string.Join(", ", columnNames)}) FROM STDIN (FORMAT BINARY)";
        
            await using (var writer = connection.BeginBinaryImport(copyCommand))
            {
                foreach (var row in dataToInsert)
                {
                    await writer.StartRowAsync();
                    foreach (var item in row)
                    {
                        await writer.WriteAsync(item);
                    }
                }

                await writer.CompleteAsync();
            }
            
            Console.WriteLine($"Successfully uploaded {dataToInsert.Count()} rows to '{tableName}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during upload: {ex.Message}");
            throw;
        }
    }
}