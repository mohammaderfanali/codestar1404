using Microsoft.Extensions.DependencyInjection;
using project;
using project.csvReader;
using project.csvReader.Abstraction;
using project.DatabaseHealthChecker.Abstraction;
using project.DataBaseUpploader;
using project.DataBaseUpploader.Abstraction;
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
        services.AddSingleton<ICsvReader, CsvReader>();
        services.AddSingleton<IPlugin, DatabasePlugin>();
        services.AddSingleton<IPlugin, CsvPlugin>();
        services.AddSingleton<IPlugin, JoinPlugin>();
        services.AddSingleton<IDatabaseHealthChecker, DatabaseHealthChecker.DatabaseHealthChecker>();
        services.AddSingleton<IDataBaseUploader, DataBaseUploader>();
        services.AddSingleton<IApplication, Application>();

        return services;
    }
    
    // private static IServiceCollection AddPlugins(this IServiceCollection services)
    // {
    //     var pluginTypes = typeof(IPlugin).Assembly
    //         .GetTypes()
    //         .Where(t => typeof(IPlugin).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
    //
    //     foreach (var type in pluginTypes)
    //     {
    //         services.AddSingleton(typeof(IPlugin), type);
    //     }
    //
    //     return services;
    // }
}

