using project.Plugins.Abstraction;
using project.Topological_sort.Models;
using project.Topological_sort;
using project.Topological_sort.GetParent;


namespace project.PluginManager;

public class PluginManager
{
    private List<IPlugin> _plugins;

    public PluginManager()
    {
        _plugins = new List<IPlugin>();
    }

    public void AddPlugin(IPlugin plugin)
    {
        _plugins.Add(plugin);
    }

   public async void  Runscenario(Graph dag)
    {
        var sorter = new TopologicalSorter();
        var parrNodes = new NodeParentProvider();
        
        var sortedNodes = sorter.Sort(dag);
        var parrents = parrNodes.GetParents(dag);
        
        List<KeyValuePair<string,string>> results = new List<KeyValuePair<string,string>>();
        
        // Console.WriteLine("Topologically sorted nodes:");
        // foreach (var node in sortedNodes)
        // {
        //     Console.WriteLine($"Node {node.Id} - Type: {node.Type}");
        // }

        foreach (var node in sortedNodes)
        {
            foreach (var plugin in _plugins)
            {
                if (plugin.Getpluginname() == node.Type)
                {
                    var parrentinput = new List<KeyValuePair<string, string>>();
                    foreach (var par in parrents[node.Id])
                    {
                        parrentinput.Add(results[par]);
                    }
                    results.Add(await plugin.Makequery(node.Data,parrentinput));
                }
            }
        }
        
        
    }
}