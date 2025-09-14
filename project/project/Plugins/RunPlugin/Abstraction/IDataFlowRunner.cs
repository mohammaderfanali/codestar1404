namespace project.Plugins.RunPlugin.Abstraction
{
    public interface IDataFlowRunner
    {
        Task RunDataFlow(DataFlow.Models.Graph dag, CancellationToken cancellationToken);
    }
}