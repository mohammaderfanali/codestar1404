using project.DataFlow.GetParent.Abstraction;
using project.DataFlow.Models.Identifier;

namespace project.DataFlow.GetParent;

public class NodeParentProvider : INodeParentProvider
{
    public Dictionary<NodeId, List<NodeId>> GetParents(Models.Graph graph)
    {
        var parents = new Dictionary<NodeId, List<NodeId>>();

        foreach (var node in graph.Nodes)
        {
            parents[node.Id] = new List<NodeId>();
        }

        foreach (var edge in graph.Edges)
        {
            parents[edge.To].Add(edge.From);
        }

        return parents;
    }
}