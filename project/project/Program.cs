using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using project.DependencyInjection;
using project.Services;
using project.Services.Abstraction;

var builder = WebApplication.CreateBuilder(args);

// --- Step 1: Add Services ---
// This adds the services needed for your API controllers
builder.Services.AddControllers();

// These two lines are REQUIRED to register the Swagger services.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// This line registers all your custom services (plugins, runners, etc.)
builder.Services.AddProjectServices();

// This registers your main logic service
builder.Services.AddScoped<IScenarioService, ScenarioService>();


// --- Step 2: Build the Application ---
var app = builder.Build();


// --- Step 3: Configure the HTTP Request Pipeline ---
// This section tells the app how to handle incoming requests.
// The order of these 'Use' calls is important.

// These two 'if' blocks enable the Swagger UI, but only in the development environment.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

// This line connects incoming requests to your Controller classes
app.MapControllers();


// --- Step 4: Run the Application ---
app.Run();