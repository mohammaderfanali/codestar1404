using Microsoft.EntityFrameworkCore;

namespace project.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<ScenarioStatus> ScenarioStatuses { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}