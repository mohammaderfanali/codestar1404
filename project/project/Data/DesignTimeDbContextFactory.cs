using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using project.Plugins;

namespace project.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(path.uploadconnection);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}