using System;
using System.Threading.Tasks;

public interface IScenarioStatusUpdater
{
    Task CreateStatusAsync(Guid scenarioId);
    Task UpdateCurrentStepAsync(Guid scenarioId, string currentStep);
    Task MarkAsCompletedAsync(Guid scenarioId);
    Task MarkAsFailedAsync(Guid scenarioId,string pluginType ,string errorMessage);
    Task MarkAsCanceledAsync(Guid scenarioId);
    Task MarkAsCancelingAsync(Guid scenarioId);
}