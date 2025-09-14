using System.Text.Json.Serialization;

namespace project.Plugins.Pluginmodels
{
    public class OutputPluginModel
    {
        [JsonPropertyName("tableName")] public string TableName { get; set; }
    }
}