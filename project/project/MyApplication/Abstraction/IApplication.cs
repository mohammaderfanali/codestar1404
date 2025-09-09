using project.PluginManager.Abstraction;

namespace project.MyApplication.Abstraction;

public interface IApplication
{
    Task RunAsync();
}