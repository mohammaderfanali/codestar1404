namespace project.Plugins.Pluginmodels;
using System.Text.Json.Serialization;

public class JoinModel
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

}