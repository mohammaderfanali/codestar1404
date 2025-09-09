namespace project.Services.Abstraction
{
    public interface IScenarioService
    {
        Task ExecuteScenarioAsync(DataFlow.Models.Graph dag, CancellationToken cancellationToken);
    }
}