namespace project.Plugins.RunPlugin.Abstraction
{
    public interface IPluginRunner
    {
        Task Runscenario(DataFlow.Models.Graph dag, CancellationToken cancellationToken);
    }
}