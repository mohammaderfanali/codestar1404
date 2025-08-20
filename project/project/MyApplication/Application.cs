using System.Text.Json;
using project;
using project.Plugins;
using project.Plugins.PluginClasses;
using project.Topological_sort;
using project.Topological_sort.Models;
using project.Topological_sort.GetParent;
namespace project.MyApplication;

public class Application
{
    public void Run()
    {
        string jsonPath = "C:\\Users\\soroo\\Desktop\\star\\" +
                          "codestar1404\\project\\project\\Plugins\\Form.json"; // Replace with your actual path
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine(path.json_not_found);
            return;
        }
        else
        {
            Console.WriteLine(path.json_found);
        }

        var json = File.ReadAllText(jsonPath);
        var dag = JsonSerializer.Deserialize<Graph>(json);
        
        var pluginmanager = new PluginManager.PluginManager();

        var dbplugin = new DatabasePlugin();
        var csvplugin = new CsvPlugin();
        
        pluginmanager.AddPlugin(dbplugin);
        pluginmanager.AddPlugin(csvplugin);

        if (dag != null) pluginmanager.Runscenario(dag);
    }
}