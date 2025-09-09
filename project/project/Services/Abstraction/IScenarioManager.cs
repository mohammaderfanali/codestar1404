using System;
using project.Graph.Models;

namespace project.Services
{
    public interface IScenarioManager
    {

        Guid StartScenario(DataFlow.Models.Graph dag);
        bool CancelScenario(Guid scenarioId);
    }
}