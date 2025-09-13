using project.DataFlow.Models.Identifier;
using project.DataFlow.Sort.Abstraction;
using project.Graph.Models;
namespace project.DataFlow.Sort;

public class TopologicalSorter : ITopologicalSorter
{
    public List<Node> Sort(DataFlow.Models.Graph dag)
    {
        var inDegree = new Dictionary<NodeId, int>();
        var graph = new Dictionary<NodeId, List<NodeId>>();

        foreach (var node in dag.Nodes)
        {
            inDegree[node.Id] = 0;
            graph[node.Id] = new List<NodeId>();
        }

        foreach (var edge in dag.Edges)
        {
            graph[edge.From].Add(edge.To);
            inDegree[edge.To]++;
        }

        var queue = new Queue<NodeId>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
        var sorted = new List<Node>();

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var currentNode = dag.Nodes.First(n => n.Id == currentId);
            sorted.Add(currentNode);

            foreach (var neighborId in graph[currentId])
            {
                inDegree[neighborId]--;
                if (inDegree[neighborId] == 0)
                    queue.Enqueue(neighborId);
            }
        }

        if (sorted.Count != dag.Nodes.Count)
            throw new InvalidOperationException("Cycle detected in DAG");

        return sorted;
    }
}