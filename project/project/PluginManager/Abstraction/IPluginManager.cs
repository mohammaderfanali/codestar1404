using project.Plugins.Abstraction;
using project.Topological_sort.Models;

namespace project.PluginManager.Abstraction;

public interface IPluginRunner
{
    void  AddPlugin(IPlugin plugin);

    public Task Runscenario(Graph dag);
}