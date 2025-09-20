using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using project.ConditionToStr;
using project.Plugins.Pluginmodels;
using Xunit;

namespace Tests.ConditionFormatTest
{
    public class ConditionFormatterTests
    {
        private readonly ConditionFormatter _formatter;

        public ConditionFormatterTests()
        {
            var logger = new NullLogger<ConditionFormatter>();
            _formatter = new ConditionFormatter(logger);
        }

        private JsonElement ToJsonElement(object value)
        {
            if (value == null)
            {
                return JsonDocument.Parse("null").RootElement;
            }

            var jsonString = JsonSerializer.Serialize(value);
            return JsonDocument.Parse(jsonString).RootElement;
        }

        [Fact]
        public void Format_WithStringEquals_ReturnsCorrectSql()
        {
            var condition = new BaseCondition
            {
                ColumnName = "name",
                Operator = ConditionOperator.Equals,
                Value = ToJsonElement("Ali")
            };
            var result = _formatter.Format(condition);
            Assert.Equal("\"name\" = 'Ali'", result);
        }

        [Fact]
        public void Format_WithIntegerGreaterThan_ReturnsCorrectSql()
        {
            var condition = new BaseCondition
            {
                ColumnName = "age",
                Operator = ConditionOperator.GreaterThan,
                Value = ToJsonElement(30)
            };
            var result = _formatter.Format(condition);
            Assert.Equal("\"age\" > 30", result);
        }

        [Fact]
        public void Format_WithBooleanEquals_ReturnsCorrectSql()
        {
            var condition = new BaseCondition
            {
                ColumnName = "is_active",
                Operator = ConditionOperator.Equals,
                Value = ToJsonElement(true)
            };

            var result = _formatter.Format(condition);

            Assert.Equal("\"is_active\" = TRUE", result);
        }

        [Fact]
        public void Format_WithNullValue_ReturnsSqlNull()
        {
            var condition = new BaseCondition
            {
                ColumnName = "manager",
                Operator = ConditionOperator.Equals,
                Value = ToJsonElement(null)
            };

            var result = _formatter.Format(condition);

            Assert.Equal("\"manager\" = NULL", result);
        }

        [Fact]
        public void Format_WithStringContainingSingleQuote_ReturnsEscapedSql()
        {
            var condition = new BaseCondition
            {
                ColumnName = "company",
                Operator = ConditionOperator.Equals,
                Value = ToJsonElement("O'Malley's")
            };

            var result = _formatter.Format(condition);

            Assert.Equal("\"company\" = 'O''Malley''s'", result);
        }

        [Fact]
        public void Format_WithUnsupportedOperator_ThrowsNotSupportedException()
        {
            var condition = new BaseCondition
            {
                ColumnName = "city",
                Operator = (ConditionOperator)99,
                Value = ToJsonElement("Tehran")
            };
            Assert.Throws<NotSupportedException>(() => _formatter.Format(condition));
        }
    }
}