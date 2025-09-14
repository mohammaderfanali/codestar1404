using System.Text.Json.Serialization;
using project.DataFlow.Models.Identifier;

namespace project.Graph.Models;

public class Edge
{
    [JsonPropertyName("from")] public NodeId From { get; set; }

    [JsonPropertyName("to")] public NodeId To { get; set; }
}