using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Xunit;
using faz3;

public class SearcherTests
{
    private static readonly Dictionary<string, List<string>> TestIndex = new()
    {
        { "apple", new List<string> { "doc1", "doc2" } },
        { "banana", new List<string> { "doc1", "doc3" } },
        { "cherry", new List<string> { "doc2", "doc4" } },
        { "date", new List<string> { "doc5" } }
    };

    private readonly ITokenizer _tokenizerMock;
    private readonly Searcher _searcher;

    public SearcherTests()
    {
        _tokenizerMock = Substitute.For<ITokenizer>();
        _searcher = new Searcher(TestIndex, _tokenizerMock);
    }

    [Theory]
    [InlineData(
        "apple banana",
        new[] { "doc1" },
        new[] { "apple", "banana" },
        new string[] { },
        new string[] { },
        "Required words only"
    )]
    [InlineData(
        "+apple -banana",
        new[] { "doc2" },
        new string[] { },
        new[] { "apple" },
        new[] { "banana" },
        "Optional and excluded words"
    )]
    [InlineData(
        "apple +banana -cherry",
        new[] { "doc1" },
        new[] { "apple" },
        new[] { "banana" },
        new[] { "cherry" },
        "All query types combined"
    )]
    [InlineData(
        "+banana  -cherry apple  ",
        new[] { "doc1" },
        new[] { "apple" },
        new[] { "banana" },
        new[] { "cherry" },
        "All query types combined"
    )]
    [InlineData(
        "missing",
        new string[] { },
        new[] { "missing" },
        new string[] { },
        new string[] { },
        "Non-existent required word"
    )]
    [InlineData(
        "",
        new[] { "doc1", "doc2", "doc3", "doc4", "doc5" },
        new string[] { },
        new string[] { },
        new string[] { },
        "Empty query returns all docs"
    )]
    public void Search_ReturnsCorrectResults(
        string query,
        string[] expected,
        string[] required,
        string[] optional,
        string[] excluded,
        string testCase)
    {
        // Arrange
        var allTokens = required
            .Concat(optional.Select(w => "+" + w))
            .Concat(excluded.Select(w => "-" + w))
            .ToList();

        _tokenizerMock.Tokenner(query).Returns(allTokens);

        // Act
        var result = _searcher.Search(query);

        // Assert
        Assert.Equal(expected.OrderBy(x => x), result.OrderBy(x => x));
    }
    
}
