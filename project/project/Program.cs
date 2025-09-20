using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using project.Data;
using project.DependencyInjection;
using project.Plugins;
using project.Services;
using project.Services.Abstraction;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProjectServices();
builder.Services.AddScoped<IScenarioService, ScenarioService>();

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(path.uploadconnection)
);
builder.Services.AddScoped<IScenarioStatusUpdater, ScenarioStatusUpdater>();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    using var dbContext = contextFactory.CreateDbContext();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
