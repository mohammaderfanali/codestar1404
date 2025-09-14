namespace project.Services.Abstraction
{
    public interface IScenarioManager
    {

        Guid StartScenario(DataFlow.Models.Graph dag);
        bool CancelScenario(Guid scenarioId);
    }
}