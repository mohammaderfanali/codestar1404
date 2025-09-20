using System.Text.Json.Serialization;

namespace project.Plugins.Pluginmodels
{
    public class AggregationModel
    {
        [JsonPropertyName("aggregationType")] public required string AggregationType { get; set; }

        [JsonPropertyName("aggregationColumn")]
        public required string AggregationColumn { get; set; }

        [JsonPropertyName("groupByColumns")] public required List<string> GroupByColumns { get; set; }
    }
}