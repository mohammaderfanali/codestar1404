using System.Text.Json;
using project.csvReader;
using project.DataBaseUpploader;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;

namespace project.Plugins.PluginClasses;

public class CsvPlugin:IPlugin
{


    public Task<KeyValuePair<string,string>> Makequery(string jsoncommanddata, string[] pastquery)
    {
        if (pastquery != null )
            throw new ArgumentException("csvreader has no pastquery");

        Console.WriteLine("Running csvreader plugin...");
        
        string connectionstring = path.uploadconnection;

        var command = JsonSerializer.Deserialize<CsvReaderModel>(jsoncommanddata);
        if (command != null)
        {
            var filepath = command.Filepath;
             var csvreader = new CsvReader();
             var tablename = csvreader.GetFileName(filepath);
             var content = csvreader.ReadCsvFile(filepath);
             
             
             var dataBaseUploader = new DataBaseUploader();
             
             dataBaseUploader.UploadDataAsync(connectionstring,
                 csvreader.GetFileName(filepath),content);

             var dbchecker = new DatabaseHealthChecker.DatabaseHealthChecker();
             var connectionTask = dbchecker.IsConnectionValidAsync(connectionstring);
             var dataTask = dbchecker.TableHasDataAsync(connectionstring, tablename);

            string query="SELECT * FROM [" + tablename + "]";
            
            Console.WriteLine("csvreader plugin completed successfully");

            return Task.FromResult(new KeyValuePair<string, string>(query, connectionstring));
                    
        }
        
        Console.WriteLine("csvreader plugin completed unsuccessfully");
        return Task.FromResult(new KeyValuePair<string, string>());
        
    }
}