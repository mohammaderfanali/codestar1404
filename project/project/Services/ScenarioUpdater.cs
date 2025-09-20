using Microsoft.EntityFrameworkCore;
using project.Data;

namespace project.Services;

public class ScenarioStatusUpdater : IScenarioStatusUpdater
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ScenarioStatusUpdater(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task CreateStatusAsync(Guid scenarioId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        if (await context.ScenarioStatuses.AnyAsync(s => s.ScenarioId == scenarioId)) return;

        var newStatus = new ScenarioStatus
        {
            ScenarioId = scenarioId,
            Status = "Running",
            CurrentStep = "Scenario initiated...",
            StartTime = DateTime.UtcNow,
            LastUpdateTime = DateTime.UtcNow
        };
        context.ScenarioStatuses.Add(newStatus);
        await context.SaveChangesAsync();
    }

    public async Task UpdateCurrentStepAsync(Guid scenarioId, string currentStep)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.ScenarioStatuses
            .Where(s => s.ScenarioId == scenarioId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.CurrentStep, currentStep)
                .SetProperty(b => b.LastUpdateTime, DateTime.UtcNow));
    }

    public async Task MarkAsCompletedAsync(Guid scenarioId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.ScenarioStatuses
            .Where(s => s.ScenarioId == scenarioId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.Status, "Completed")
                .SetProperty(b => b.CurrentStep, "Scenario finished successfully.")
                .SetProperty(b => b.LastUpdateTime, DateTime.UtcNow));
    }

    public async Task MarkAsFailedAsync(Guid scenarioId, string pluginName, string errorMessage)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.ScenarioStatuses
            .Where(s => s.ScenarioId == scenarioId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.Status, "Failed")
                .SetProperty(b => b.CurrentStep, $"Plugin '{pluginName}' failed: {errorMessage}")
                .SetProperty(b => b.LastUpdateTime, DateTime.UtcNow));
    }



    public async Task MarkAsCanceledAsync(Guid scenarioId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.ScenarioStatuses
            .Where(s => s.ScenarioId == scenarioId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.Status, "Canceled")
                .SetProperty(b => b.CurrentStep, "Scenario was canceled by user.")
                .SetProperty(b => b.LastUpdateTime, DateTime.UtcNow));
    }

    public async Task MarkAsCancelingAsync(Guid scenarioId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.ScenarioStatuses
            .Where(s => s.ScenarioId == scenarioId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.Status, "Canceling")
                .SetProperty(b => b.CurrentStep, "Cancellation request received. Waiting for the process to stop...")
                .SetProperty(b => b.LastUpdateTime, DateTime.UtcNow));
    }
}