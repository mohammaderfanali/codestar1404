using Microsoft.Extensions.DependencyInjection;
using project;
using project.ConditionToStr;
using project.ConditionToStr.Abstraction;
using project.DataBase.CreateTableFromQuery;
using project.DataBase.CreateTableFromQuery.Abstraction;
using project.DataBase.CreateTableQuery;
using project.DataBase.DatabaseHealthChecker.Abstraction;
using project.DataBase.DataBaseUpploader;
using project.DataBase.DataTableUpploader.Abstraction;
using project.DataBase.QueryExecutor;
using project.DataBase.QueryExecutor.Abstraction;
using project.DataFlow.GetParent;
using project.DataFlow.GetParent.Abstraction;
using project.DataFlow.Sort;
using project.DataFlow.Sort.Abstraction;
using project.Helpers.ColumnNameMaker;
using project.Helpers.ColumnNameMaker.Abstraction;
using project.Helpers.ReadCsv;
using project.Plugins.Abstraction;
using project.Plugins.PluginClasses;
using project.Plugins.Pluginmodels;
using project.Plugins.RunPlugin;
using project.Plugins.RunPlugin.Abstraction;
using project.ReadCsv;
using project.ReadCsv.Abstraction;
using project.Services;
using project.Services.Abstraction;

namespace project.DependencyInjection;

public static class DependencyRegistrationExtensions
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddSingleton<IDataFlowRunner, DataFlowRunner>();
        services.AddSingleton<ICsvReader, CsvReader>();
        services.AddSingleton<IPlugin, DatabasePlugin>();
        services.AddSingleton<IPlugin, CsvPlugin>();
        services.AddSingleton<IPlugin, JoinPlugin>();
        services.AddSingleton<IPlugin, OutputPlugin>();
        services.AddSingleton<IPlugin, AggregationPlugin>();
        services.AddSingleton<IScenarioManager, ScenarioManager>();
        services.AddSingleton<IPlugin, FilterPlugin>();
        services.AddSingleton<IConditionFormatter, ConditionFormatter>();
        services.AddSingleton<IQueryTableGenerator, QueryTableGenerator>();
        services.AddSingleton<IColumnNameResolver, ColumnNameResolver>();
        services.AddSingleton<ITopologicalSorter, TopologicalSorter>();
        services.AddSingleton<INodeParentProvider, NodeParentProvider>();


        






        services.AddSingleton<IDatabaseHealthChecker, DataBase.DatabaseHealthChecker.DatabaseHealthChecker>();
        services.AddSingleton<IDataTableUplouder, DataTableUplouder>();
        services.AddSingleton<ITransferTable, TransferTable>();
        services.AddSingleton<ISelectQueryExecutor, SelectSelectQueryExecutor>();


        return services;
    }
}