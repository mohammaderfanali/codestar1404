using System.Text.Json;
using project;
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
            Console.WriteLine("JSON file not found.");
            return;
        }
        else
        {
            Console.WriteLine("JSON file found.");
        }

        var json = File.ReadAllText(jsonPath);
        var dag = JsonSerializer.Deserialize<Graph>(json);

        var sorter = new TopologicalSorter();
        var parrNodes = new NodeParentProvider();
        

            var sortedNodes = sorter.Sort(dag);
            var parrents = parrNodes.GetParents(dag);
            Console.WriteLine("Topologically sorted nodes:");
            foreach (var node in sortedNodes)
            {
                Console.WriteLine($"Node {node.Id} - Type: {node.Type}");
            }

        
        List<KeyValuePair<string,string>> results = new List<KeyValuePair<string,string>>();
        // foreach (var node in sortedNodes)
        // {
        //     
        // }
        
        
        
    }
}