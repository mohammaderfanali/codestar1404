using project.Topological_sort.Models;
using System.Threading.Tasks;

namespace project.PluginManager.Abstraction
{
    public interface IPluginRunner
    {
        Task Runscenario(Graph dag);
    }
}