namespace project.Plugins.RunPlugin.Abstraction
{
    public interface IDataFlowRunner
    {
        Task RunDataFlow(Guid scenarioId ,DataFlow.Models.Graph dag, CancellationToken cancellationToken);
    }
}