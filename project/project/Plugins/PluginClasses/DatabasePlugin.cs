using System.Text.Json;
using project.csvReader;
using project.DataBaseUpploader;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;
using project.DatabaseHealthChecker;
namespace project.Plugins.PluginClasses;

public class DatabasePlugin:IPlugin
{
    
    public async Task<KeyValuePair<string,string>> Makequery(JsonElement commandelement,
        List<KeyValuePair<string, string>> pastquery = null)
    {
        if (pastquery.Count!=0)
            throw new ArgumentException("DatabaseReader has no pastquery");
        string jsoncommanddata = commandelement.GetRawText();
        Console.WriteLine(path.running_database_plugin);

        string connectionstring = path.uploadconnection;

        var command = JsonSerializer.Deserialize<DatabaseConnectionModel>(jsoncommanddata);
        if (command == null)
        {
            Console.WriteLine(path.DatabasePlugin_Makequery_empty_command);
            Console.WriteLine(path.complete_unsuccesfuly);
            return new KeyValuePair<string, string>();

        }
        string tableName = command.Tablename;
        string query = $"SELECT * FROM [{tableName}];";
        
        var checker = new DatabaseHealthChecker.DatabaseHealthChecker();
        if (await checker.IsConnectionValidAsync(connectionstring))
        {
            if (!await checker.TableHasDataAsync(connectionstring, tableName))
            {
                Console.WriteLine(path.empty_table);
            }
    
            Console.WriteLine(path.complete_succesfuly);
            return new KeyValuePair<string, string>(query, connectionstring);
        }
        Console.WriteLine(path.complete_unsuccesfuly);
        return new KeyValuePair<string, string>();
    }

    public string Getpluginname()
    {
        return "databasereader";
    }
}
