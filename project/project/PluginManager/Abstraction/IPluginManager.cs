using project.Plugins.Abstraction;
using project.Topological_sort.Models;

namespace project.PluginManager.Abstraction;

public interface IPluginManager
{
    void  Addplugin(IPlugin plugin);
    void Runscenario(Graph dag);

}