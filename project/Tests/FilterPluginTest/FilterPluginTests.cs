using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using project.ConditionToStr;
using project.ConditionToStr.Abstraction;
using project.Models.pluginoutput;
using project.Plugins.PluginClasses;
using Xunit;

namespace Tests.FilterPluginTest
{
    public class CorrectedFilterPluginTest
    {
        private readonly FilterPlugin _plugin;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public CorrectedFilterPluginTest()
        {
            var logger = new NullLogger<FilterPlugin>();
            var logger2 = new NullLogger<ConditionFormatter>();

            var conditionFormatter = new ConditionFormatter(logger2);
            _plugin = new FilterPlugin(logger, conditionFormatter);
        }

        private JsonElement CreateCommandFromFile(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            return JsonDocument.Parse(jsonString).RootElement;
        }

        private List<PluginOutput> CreateParentOutput(string query = "SELECT * FROM initial_data")
        {
            return new List<PluginOutput> { new PluginOutput(query, "dummy_connection_string") };
        }

        
        [Fact]
        public async Task Makequery_WithAndOfOrsFilter_GeneratesCorrectSqlQuery()
        {

            var jsonElement = CreateCommandFromFile("../../../FilterPluginTest/test.json");
            var parentOutputs = CreateParentOutput();

            var result = await _plugin.Makequery(jsonElement, _cancellationToken, parentOutputs);

            var expectedQuery = "SELECT * FROM (SELECT * FROM initial_data)  WHERE (\"name\" = 'Ali' OR \"lastName\" = 'Alavi') AND (\"age\" > 25)";
            Assert.Equal(expectedQuery, result.Query);
        }
    }
}