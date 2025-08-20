using System.Text.Json;
using project.csvReader;
using project.DataBaseUpploader;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;

namespace project.Plugins.PluginClasses;

public class CsvPlugin:IPlugin
{


    public Task<KeyValuePair<string,string>> Makequery(JsonElement commandelement,
        List<KeyValuePair<string, string>> pastquery)
    {
        if (pastquery.Count!=0)
            throw new ArgumentException("csvreader has no pastquery");

        string jsoncommanddata = commandelement.GetRawText();
        
        Console.WriteLine(path.running_csvreader);
        
        string connectionstring = path.uploadconnection;

        var command = JsonSerializer.Deserialize<CsvReaderModel>(jsoncommanddata);
        if (command != null)
        {
            var filepath = command.Filepath;
             var csvreader = new CsvReader();
             var tablename = csvreader.GetFileName(filepath);
             var content = csvreader.ReadCsvFile(filepath);
             
             
             var dataBaseUploader = new DataBaseUploader();
             
             _ = dataBaseUploader.UploadDataAsync(connectionstring,
                 csvreader.GetFileName(filepath),content);

             var dbchecker = new DatabaseHealthChecker.DatabaseHealthChecker();
             var connectionTask = dbchecker.IsConnectionValidAsync(connectionstring);
             var dataTask = dbchecker.TableHasDataAsync(connectionstring, tablename);

            string query="SELECT * FROM [" + tablename + "]";
            
            Console.WriteLine(path.csvreader_complete_succesfuly);

            return Task.FromResult(new KeyValuePair<string, string>(query, connectionstring));
                    
        }
        
        Console.WriteLine(path.csvreader_complete_unsuccesfuly);
        return Task.FromResult(new KeyValuePair<string, string>());
        
    }

    public string Getpluginname()
    {
        return "csvreader";
    }
    
}
