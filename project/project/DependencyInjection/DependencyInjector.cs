using Microsoft.Extensions.DependencyInjection;
using project;
using project.MyApplication;
using project.MyApplication.Abstraction;
using project.PluginManager;
using project.PluginManager.Abstraction;
using project.Plugins;
using project.Plugins.Abstraction;
using project.Plugins.PluginClasses;

namespace project.DependencyInjection;

public static class DependencyRegistrationExtensions
{
   
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {

        services.AddSingleton<IPluginRunner, PluginRunner>();
        
        services.AddSingleton<IPlugin, DatabasePlugin>();
        services.AddSingleton<IPlugin, CsvPlugin>();
        services.AddSingleton<IApplication, Application>();

        return services;
    }
}

