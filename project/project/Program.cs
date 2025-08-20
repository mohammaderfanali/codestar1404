using project.MyApplication;

namespace project;

class Program
{
    static async Task Main(string[] args)
    {
        // string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=project";
        // string filePath = "C:/Users/soroo/Downloads/countries.csv"; // مسیر فایل CSV
        // string tableName = "countries";
        //
        // var importer = new CsvReader();
        // var content = importer.ReadCsvFile(filePath);
        //
        // foreach (var row in content)
        // {
        //     Console.WriteLine(string.Join(" | ", row));
        // }
        //
        // var uploader = new DataBaseUploader();
        // await uploader.UploadDataAsync(connectionString, tableName,content);
        //
        // var dbvalid = new DatabaseHealthChecker();
        // Console.WriteLine(await dbvalid.IsConnectionValidAsync(connectionString));
        // Console.WriteLine(await dbvalid.TableHasDataAsync(connectionString, tableName));
        //
        // var connectdb = new DatabasePlugin();
        var application = new Application();
        application.Run();
    }
}
