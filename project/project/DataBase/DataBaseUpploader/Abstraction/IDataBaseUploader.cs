using Microsoft.Extensions.Logging;

namespace project.DataBaseUpploader.Abstraction;

public interface IDataBaseUploader
{
    Task UploadDataAsync(
        string connectionString, 
        string tableName, 
        string[] columnNames,
        List<string[]> data,
        CancellationToken cancellationToken =  default);
}
