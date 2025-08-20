using System.Text.Json.Serialization;

namespace project.Topological_sort.Models;

public class Graph
{
    [JsonPropertyName("nodes")]
    public required List<Node> Nodes { get; set; }

    [JsonPropertyName("edges")]
    public required List<Edge> Edges { get; set; }
}