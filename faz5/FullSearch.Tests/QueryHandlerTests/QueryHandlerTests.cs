using FluentAssertions;
using Xunit;

namespace SearchEngine.Tests;

public class QueryHandlerTests
{
    private readonly QueryHandler _handler;

    public QueryHandlerTests()
    {
        _handler = new QueryHandler();
    }

    public static IEnumerable<object[]> TestQueries()
    {
        yield return new object[]
        {
            "hello world",
            new List<(string value, char prefix)> { ("hello", '\0'), ("world", '\0') }
        };

        yield return new object[]
        {
            "+hello +world",
            new List<(string value, char prefix)> { ("hello", '+'), ("world", '+') }
        };

        yield return new object[]
        {
            "-hello -world",
            new List<(string value, char prefix)> { ("hello", '-'), ("world", '-') }
        };
        
        yield return new object[]
        {
            "search +engine -java",
            new List<(string value, char prefix)> { ("search", '\0'), ("engine", '+'), ("java", '-') }
        };

        yield return new object[]
        {
            "\"hello beautiful world\"",
            new List<(string value, char prefix)> { ("hello beautiful world", '\0') }
        };
        
        yield return new object[]
        {
            "info +\"phrase search\" -data",
            new List<(string value, char prefix)> { ("info", '\0'), ("phrase search", '+'), ("data", '-') }
        };
        
        
        yield return new object[]
        {
            "",
            new List<(string value, char prefix)>()
        };
        
        yield return new object[]
        {
            "HELLO -World",
            new List<(string value, char prefix)> { ("hello", '\0'), ("world", '-') }
        };
    }

    [Theory]
    [MemberData(nameof(TestQueries))]
    public void SplitCommandWithRegex_ShouldParseVariousQueriesCorrectly(string query, List<(string value, char prefix)> expected)
    {
       

        // Act
        var actual = _handler.SplitCommandWithRegex(query);

        // Assert
        actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}