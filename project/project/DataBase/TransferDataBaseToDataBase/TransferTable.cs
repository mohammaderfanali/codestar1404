using System.Data;
using Microsoft.Extensions.Logging;
using project.DataBase.CreateTableFromQuery.Abstraction;
using project.DataBase.DataTableUpploader.Abstraction;
using project.DataBase.QueryExecutor.Abstraction;

namespace project.DataBase.CreateTableFromQuery
{
    public class TransferTable : ITransferTable
    {
        private readonly ILogger<TransferTable> _logger;
        private readonly ISelectQueryExecutor _selectQueryExecutor;
        private readonly IDataTableUplouder _dataTableUplouder;

        public TransferTable(
            ILogger<TransferTable> logger,
            ISelectQueryExecutor selectQueryExecutor,
            IDataTableUplouder dataTableUplouder)
        {
            _logger = logger;
            _selectQueryExecutor = selectQueryExecutor;
            _dataTableUplouder = dataTableUplouder;
        }

        public async Task Transfer(
            string sourceQuery,
            string sourceConnectionString,
            string destinationConnectionString,
            string newTableName,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting transfer of query result to new table '{NewTableName}'.", newTableName);

            var dataTable = await _selectQueryExecutor.ExecuteQueryAsync(sourceQuery, sourceConnectionString, cancellationToken);

            if (dataTable == null || dataTable.Columns.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Could not derive schema for table '{newTableName}'. The query returned no columns.");
            }

            dataTable.TableName = newTableName;

            try
            {
                await _dataTableUplouder.UploadDataAsync(destinationConnectionString, dataTable, cancellationToken);
                _logger.LogInformation("Successfully transferred data to table '{NewTableName}'.", newTableName);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Transfer to table '{NewTableName}' was canceled.", newTableName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to transfer data to table '{NewTableName}'.", newTableName);
                throw;
            }
        }
    }
}
