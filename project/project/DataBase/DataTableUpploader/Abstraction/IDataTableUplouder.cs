using System.Data;

namespace project.DataBase.DataTableUpploader.Abstraction;

public interface IDataTableUplouder
{
    Task UploadDataAsync(
        string connectionString, 
        DataTable table,
        CancellationToken cancellationToken =  default);
}
