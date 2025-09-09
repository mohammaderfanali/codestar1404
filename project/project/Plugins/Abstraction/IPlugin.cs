using System.Text.Json;
using System.Text.Json.Serialization;

namespace project.Plugins.Abstraction;

public interface IPlugin
{
    string PluginName { get; }
    public Task<KeyValuePair<string, string>> Makequery(
        JsonElement commandelement,
        List<KeyValuePair<string, string>> pastqueroes = null);

}
