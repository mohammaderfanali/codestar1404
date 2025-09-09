using Microsoft.Extensions.DependencyInjection;
using project;
using project.CreateTableFromQuery;
using project.CreateTableFromQuery.Abstraction;
using project.DatabaseHealthChecker.Abstraction;
using project.DataBaseUpploader;
using project.DataBaseUpploader.Abstraction;
using project.MyApplication;
using project.MyApplication.Abstraction;
using project.Plugins;
using project.Plugins.Abstraction;
using project.Plugins.PluginClasses;
using project.Plugins.Pluginmodels;
using project.Plugins.RunPlugin;
using project.Plugins.RunPlugin.Abstraction;
using project.ReadCsv;
using project.ReadCsv.Abstraction;
using project.TransferTablefromQuery;
using project.TransferTablefromQuery.Abstraction;

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
        services.AddSingleton<IPlugin, OutputPlugin>();
        services.AddSingleton<IPlugin, AggregationPlugin>();


        services.AddSingleton<IDatabaseHealthChecker, DatabaseHealthChecker.DatabaseHealthChecker>();
        services.AddSingleton<IDataBaseUploader, DataBaseUploader>();
        services.AddSingleton<IApplication, Application>();
        services.AddSingleton<ITableCreator, TableCreator>();
        services.AddSingleton<IDataInserter, DataInserter>();
        
        return services;
    }
}