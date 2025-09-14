using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using project.Models.pluginoutput;

namespace project.Plugins.Abstraction
{
    public interface IPlugin
    {
        string PluginName { get; }

        Task<PluginOutput> Makequery(JsonElement commandelement, CancellationToken cancellationToken,
            List<PluginOutput> pastOutputs = null);
    }
}