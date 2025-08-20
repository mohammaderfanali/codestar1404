using System.Text.Json.Serialization;

namespace project.Plugins.Pluginmodels;

public class CsvReaderModel
{
    [JsonPropertyName("filepath")]
    public required string Filepath { get; set; }
}