using System.Text.Json.Serialization;

namespace project.Plugins.Abstraction;

public interface IPlugin
{
    Task<KeyValuePair<string,string>> Makequery(string jsoncommanddata,string[] pastquery=null);
}