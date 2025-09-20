using System.Text.Json;
using Microsoft.Extensions.Logging;
using NSubstitute;
using project.Models.pluginoutput;
using project.Plugins.PluginClasses;
using project.Plugins.Pluginmodels;
using Xunit;

namespace Tests.AggregationPluginTest;

public class AggregationPluginTests
{
    private readonly AggregationPlugin _plugin;

    public AggregationPluginTests()
    {
        var logger = Substitute.For<ILogger<AggregationPlugin>>();
        _plugin = new AggregationPlugin(logger);
    }

    [Fact]
    public async Task Makequery_WithValidInput_ReturnsExpectedQuery()
    {
        // Arrange
        var command = new AggregationModel
        {
            AggregationType = "SUM",
            AggregationColumn = "Amount",
            GroupByColumns = new List<string> { "Category", "Region" }
        };
        var json = JsonSerializer.Serialize(command);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        var parentOutput = new PluginOutput("SELECT * FROM Sales", "conn-string");
        var parentOutputs = new List<PluginOutput> { parentOutput };

        // Act
        var result = await _plugin.Makequery(jsonElement, CancellationToken.None, parentOutputs);

        // Assert
        var expectedQuery =
            "SELECT \"Category\", \"Region\", SUM(\"Amount\") FROM (SELECT * FROM Sales) GROUP BY \"Category\", \"Region\"";
        Assert.Equal(expectedQuery, result.Query);
        Assert.Equal("conn-string", result.ConnectionString);
    }

    [Fact]
    public async Task Makequery_WithNoParentOutput_ThrowsArgumentException()
    {
        var command = new AggregationModel
        {
            AggregationType = "COUNT",
            AggregationColumn = "Id",
            GroupByColumns = new List<string> { "Type" }
        };
        var json = JsonSerializer.Serialize(command);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _plugin.Makequery(jsonElement, CancellationToken.None, new List<PluginOutput>()));
    }

    [Fact]
    public async Task Makequery_WithInvalidCommand_ThrowsArgumentException()
    {
        var invalidCommand = new AggregationModel
        {
            AggregationType = "INVALID",
            AggregationColumn = "",
            GroupByColumns = new List<string>()
        };
        var json = JsonSerializer.Serialize(invalidCommand);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        var parentOutput = new PluginOutput("SELECT * FROM Table", "conn");
        var parentOutputs = new List<PluginOutput> { parentOutput };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _plugin.Makequery(jsonElement, CancellationToken.None, parentOutputs));
    }

    [Fact]
    public async Task Makequery_WithEmptyParentQuery_ThrowsInvalidOperationException()
    {
        var command = new AggregationModel
        {
            AggregationType = "MAX",
            AggregationColumn = "Score",
            GroupByColumns = new List<string> { "Level" }
        };
        var json = JsonSerializer.Serialize(command);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        var parentOutput = new PluginOutput("", "conn");
        var parentOutputs = new List<PluginOutput> { parentOutput };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _plugin.Makequery(jsonElement, CancellationToken.None, parentOutputs));
    }

    [Fact]
    public async Task Makequery_WhenCancelled_ThrowsOperationCanceledException()
    {
        var command = new AggregationModel
        {
            AggregationType = "AVG",
            AggregationColumn = "Duration",
            GroupByColumns = new List<string> { "Session" }
        };
        var json = JsonSerializer.Serialize(command);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        var parentOutput = new PluginOutput("SELECT * FROM Logs", "conn");
        var parentOutputs = new List<PluginOutput> { parentOutput };

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _plugin.Makequery(jsonElement, cts.Token, parentOutputs));
    }
}