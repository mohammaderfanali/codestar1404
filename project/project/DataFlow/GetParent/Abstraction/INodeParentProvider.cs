using project.DataFlow.Models.Identifier;

namespace project.DataFlow.GetParent.Abstraction;

public interface INodeParentProvider
{
    public Dictionary<NodeId, List<NodeId>> GetParents(Models.Graph graph);
}