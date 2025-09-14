using System.Data;

namespace project.DataBase.DataBaseUpploader.Abstraction;

public interface IDataBaseUploader
{
    Task UploadDataAsync(
        string connectionString, 
        DataTable table,
        CancellationToken cancellationToken =  default);
}
