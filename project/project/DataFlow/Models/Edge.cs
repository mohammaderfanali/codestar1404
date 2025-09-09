using System.Text.Json.Serialization;

namespace project.Graph.Models;

public class Edge
{
    [JsonPropertyName("from")]
    public int From { get; set; }

    [JsonPropertyName("to")]
    public int To { get; set; }


}