namespace project.Services.Abstraction
{
    public interface IScenarioService
    {
        Task ExecuteScenarioAsync(Guid scenarioId,DataFlow.Models.Graph dag, CancellationToken cancellationToken);
    }
}