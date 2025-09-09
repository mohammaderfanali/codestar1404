using System.Text.Json;
using project.Plugins.Abstraction;
using project.Plugins.Pluginmodels;
namespace project.Plugins.PluginClasses;

public class JoinPlugin : IPlugin
{
    public string PluginName { get; }

    public JoinPlugin()
    {
        PluginName = "JoinPlugin";
    }

    public async Task<KeyValuePair<string, string>> Makequery(JsonElement commandelement,
        List<KeyValuePair<string, string>> pastquery = null)
    {
        Console.WriteLine(path.running_join);
        if (pastquery.Count==0)
            throw new ArgumentException("JoinPlugin has pastquery");
        string jsoncommanddata = commandelement.GetRawText();

        var command = JsonSerializer.Deserialize<JoinModel>(jsoncommanddata);
        var type = command.Type;
        
        
        
        return new KeyValuePair<string, string>(path.running_join, jsoncommanddata);
    }
    
}