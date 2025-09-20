using System.Text.Json.Serialization;

namespace project.Plugins.Pluginmodels
{
    public class FilterPluginModel
    {

        [JsonPropertyName("filterGroups")]
        public required List<OrFilterGroup> FilterGroups { get; set; }
    }

    public class OrFilterGroup
    {
        [JsonPropertyName("conditions")]
        public required List<BaseCondition> Conditions { get; set; }
    }

    public class BaseCondition
    {
        [JsonPropertyName("columnName")]
        public required string ColumnName { get; set; }

        [JsonPropertyName("operator")]
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public required ConditionOperator Operator { get; set; }

        [JsonPropertyName("value")]
        public required object Value { get; set; }
    }

    public enum ConditionOperator
    {
        Equals,
        LessThan,
        GreaterThan
    }
}