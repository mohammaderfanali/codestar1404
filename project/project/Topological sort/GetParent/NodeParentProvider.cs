using project.Topological_sort.Models;

namespace project.Topological_sort.GetParent;


public class NodeParentProvider
{
    public Dictionary<int, List<int>> GetParents(Graph graph)
    {
        var parents = new Dictionary<int, List<int>>();

        foreach (var node in graph.Nodes)
        {
            parents[node.Id] = new List<int>();
        }

        foreach (var edge in graph.Edges)
        {
            parents[edge.To].Add(edge.From);
        }

        return parents;
    }
}