using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using project.Graph.Models;
using project.Services;
using project.Services.Abstraction;

namespace project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScenarioController : ControllerBase
    {
        private readonly IScenarioManager _scenarioManager;

        public ScenarioController(IScenarioManager scenarioManager)
        {
            _scenarioManager = scenarioManager;
        }

        [HttpPost("run")]
        public IActionResult RunScenario([FromBody] DataFlow.Models.Graph dag)
        {
            if (dag == null)
            {
                return BadRequest("Scenario graph cannot be null.");
            }

            var scenarioId = _scenarioManager.StartScenario(dag);
            return Ok(new { scenarioId });
        }

        [HttpPost("cancel/{scenarioId}")]
        public IActionResult CancelScenario(Guid scenarioId)
        {
            if (_scenarioManager.CancelScenario(scenarioId))
            {
                return Ok(new { message = $"Scenario {scenarioId} cancellation requested." });
            }

            return NotFound(new { message = $"Scenario with ID {scenarioId} not found or already completed." });
        }
    }
}