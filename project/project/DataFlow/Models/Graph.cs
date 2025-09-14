using System.Text.Json.Serialization;
using project.Graph.Models;

namespace project.DataFlow.Models
{
    public class Graph
    {
        [JsonPropertyName("nodes")] public required List<Node> Nodes { get; set; }

        [JsonPropertyName("edges")] public required List<Edge> Edges { get; set; }
    }
}