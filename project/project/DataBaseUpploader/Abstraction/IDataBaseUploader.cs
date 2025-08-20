namespace project.DataBaseUpploader.Abstraction;

public interface IDataBaseUploader
{
    Task UploadDataAsync(
        string connectionString, 
        string tableName, 
        List<string[]> data, 
        bool skipHeaderRow = true);
}