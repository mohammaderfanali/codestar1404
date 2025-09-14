using project.Graph.Models;

namespace project.DataFlow.Sort.Abstraction;

public interface ITopologicalSorter
{
    public List<Node> Sort(DataFlow.Models.Graph dag);
}