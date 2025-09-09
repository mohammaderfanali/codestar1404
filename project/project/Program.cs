using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using project.DependencyInjection;
using project.MyApplication.Abstraction;

namespace project
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddProjectServices();
            var host = builder.Build();

            var application = host.Services.GetRequiredService<IApplication>();
            await application.RunAsync();
        }
    }
}