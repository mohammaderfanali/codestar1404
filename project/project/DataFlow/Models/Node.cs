using System.Text.Json;
using System.Text.Json.Serialization;
using project.DataFlow.Models.Identifier;

namespace project.Graph.Models;

public class Node
{
    [JsonPropertyName("id")] public required NodeId Id { get; set; }

    [JsonPropertyName("type")] public required string Type { get; set; }

    [JsonPropertyName("data")] public JsonElement Data { get; set; } = new();
}