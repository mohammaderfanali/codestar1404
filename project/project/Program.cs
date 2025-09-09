using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using project.DependencyInjection;
using project.DependencyInjection;
using project.MyApplication;
using project.DependencyInjection;
using project.MyApplication.Abstraction;

namespace project;

class Program
{
    static void Main(string[] args)
    {
  
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddProjectServices();
        var host = builder.Build();
        var application = host.Services.GetRequiredService<IApplication>();
        application.Run();
    }
}