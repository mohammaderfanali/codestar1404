using System.Text.Json;
using Microsoft.Extensions.Logging;
using project.ConditionToStr.Abstraction;
using project.Plugins.Pluginmodels;

namespace project.ConditionToStr
{
    public class ConditionFormatter : IConditionFormatter
    {
        private readonly ILogger<ConditionFormatter> _logger;

        public ConditionFormatter(ILogger<ConditionFormatter> logger)
        {
            _logger = logger;
        }

        public string Format(BaseCondition condition)
        {
            var columnName = $"\"{condition.ColumnName.Trim()}\"";
            var op = condition.Operator switch
            {
                ConditionOperator.Equals => "=",
                ConditionOperator.LessThan => "<",
                ConditionOperator.GreaterThan => ">",
                _ => throw new NotSupportedException($"Operator '{condition.Operator}' is not supported.")
            };
            var value = FormatValue(condition.Value);
            return $"{columnName} {op} {value}";
        }
        private string FormatValue(object value)
        {
            try
            {
                if ((value is JsonElement jsonElem && jsonElem.ValueKind == JsonValueKind.Null))
                {
                    return "NULL";
                }

                if (value is JsonElement jsonElement)
                {
                    return jsonElement.ValueKind switch
                    {
                        JsonValueKind.String => FormatObjectAsString(jsonElement.GetString() ?? throw new InvalidOperationException()),
                        JsonValueKind.Number => jsonElement.GetRawText(),
                        JsonValueKind.True or JsonValueKind.False => jsonElement.GetBoolean().ToString().ToUpper(),
                        _ => FormatObjectAsString(jsonElement.ToString())
                    };
                }
            
                return value switch
                {
                    string s => FormatObjectAsString(s),
                    int or long or double or float or decimal => value.ToString(),
                    bool b => b.ToString().ToUpper(),
                    DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss}'",
                    _ => FormatObjectAsString(value)
                } ?? throw new InvalidOperationException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to format a value for SQL query. The problematic value was: {Value}", value);
                throw new InvalidOperationException("Failed to format a value for the SQL query. See inner exception for details.", ex);
            }
        }

        private string FormatObjectAsString(object obj)
        {
            return $"'{obj.ToString()?.Replace("'", "''")}'";
        }
    }
}
