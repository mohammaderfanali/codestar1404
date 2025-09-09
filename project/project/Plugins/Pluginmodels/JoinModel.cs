namespace project.Plugins.Pluginmodels;
using System.Text.Json.Serialization;

public class JoinModel
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    
    [JsonPropertyName("onfirst")]
    public required string OnFirst { get; set; }
    [JsonPropertyName("onsecond")]
    public required string OnSecond { get; set; }

}