using System.Text.Json;
using System.Text.Json.Serialization;

namespace project.Graph.Models;

public class Node
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; } = new();
}