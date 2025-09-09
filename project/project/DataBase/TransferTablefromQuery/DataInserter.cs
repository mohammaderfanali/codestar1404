using Microsoft.Extensions.Logging;
using Npgsql;
using project.TransferTablefromQuery.Abstraction;

namespace project.TransferTablefromQuery
{
    public class DataInserter : IDataInserter
    {
        private readonly ILogger<DataInserter> _logger;

        public DataInserter(ILogger<DataInserter> logger)
        {
            _logger = logger;
        }

        public async Task TransferDataAsync(string sourceConnectionString, string sourceQuery, string destinationConnectionString, string newTableName)
        {
            _logger.LogInformation("Starting data transfer from source to destination table '{TableName}'.", newTableName);

            try
            {
                await using var sourceConnection = new NpgsqlConnection(sourceConnectionString);
                await sourceConnection.OpenAsync();
                await using var cmd = new NpgsqlCommand(sourceQuery, sourceConnection);
                await using var reader = await cmd.ExecuteReaderAsync();

                await using var destConnection = new NpgsqlConnection(destinationConnectionString);
                await destConnection.OpenAsync();

                // Sanitize column names to handle duplicates from the source query (e.g., from a JOIN)
                var columnNames = new List<string>();
                var uniqueColumnNames = new HashSet<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var originalName = reader.GetName(i);
                    var sanitizedName = originalName;
                    int suffix = 1;
                    while (!uniqueColumnNames.Add(sanitizedName))
                    {
                        sanitizedName = $"{originalName}_{suffix++}";
                    }
                    columnNames.Add($"\"{sanitizedName}\"");
                }

                var copyCommand = $"COPY \"{newTableName}\" ({string.Join(", ", columnNames)}) FROM STDIN (FORMAT BINARY)";

                await using var writer = destConnection.BeginBinaryImport(copyCommand);

                long rowsImported = 0;
                while (await reader.ReadAsync())
                {
                    await writer.StartRowAsync();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        await writer.WriteAsync(reader.GetValue(i));
                    }
                    rowsImported++;
                }

                await writer.CompleteAsync();

                _logger.LogInformation("Successfully transferred {RowCount} rows to table '{TableName}'.", rowsImported, newTableName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the data transfer to table '{TableName}'.", newTableName);
                throw;
            }
        }
    }
}

