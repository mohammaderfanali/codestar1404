using System;
using System.Collections.Generic;
using Xunit;
using faz3;

public class AddOptionalWordsToIndexTests
{
    // Fixed inverted index for all tests
    private static readonly Dictionary<string, List<string>> FixedIndex = new()
    {
        { "apple", new List<string> { "doc1", "doc2", "doc3" } },
        { "banana", new List<string> { "doc1", "doc3" } },
        { "cherry", new List<string> { "doc2", "doc4" } },
        { "date", new List<string> { "doc5" } }
    };

    private readonly AddOptionalWordsToIndex _indexOperator = new();

    [Theory]
    [InlineData(
        new string[] { },              // optional words
        new string[] { "doc1", "doc2" }, // input docs
        new string[] { "doc1", "doc2" }, // expected (returns input unchanged)
        "Empty optional words"
    )]
    [InlineData(
        new[] { "apple" },             // optional words
        new string[] { "doc1", "doc2", "doc5" }, // input docs
        new string[] { "doc1", "doc2" }, // expected (only docs with apple)
        "Single optional word"
    )]
    [InlineData(
        new[] { "banana", "cherry" },  // optional words
        new string[] { "doc1", "doc2", "doc3", "doc4" }, // input docs
        new string[] { "doc1", "doc2", "doc3","doc4" }, // expected (union of optional words)
        "Multiple optional words"
    )]
    [InlineData(
        new[] { "date" },              // optional words
        new string[] { "doc1", "doc5" }, // input docs
        new string[] { "doc5" },       // expected
        "Single match with optional"
    )]
    [InlineData(
        new[] { "missing" },           // optional words
        new string[] { "doc1", "doc2" }, // input docs
        new string[] { },              // expected (empty - no matches)
        "Non-existent optional word"
    )]
    [InlineData(
        new[] { "apple", "missing" },  // optional words
        new string[] { "doc1", "doc2", "doc5" }, // input docs
        new string[] { "doc1", "doc2" }, // expected (only apple matches)
        "Mixed existent/non-existent optional words"
    )]
    public void Operation_ReturnsCorrectDocuments(
        string[] optional,
        string[] docs,
        string[] expected,
        string testName)
    {
        // Act
        var result = _indexOperator.Operation(
            FixedIndex,
            new List<string>(optional),
            new List<string>(docs));

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Operation_ReturnsEmpty_WhenNoMatches()
    {
        // Arrange
        var optional = new List<string> { "date" };
        var docs = new List<string> { "doc1", "doc2" };

        // Act
        var result = _indexOperator.Operation(FixedIndex, optional, docs);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Operation_ReturnsOriginalDocs_WhenEmptyOptional()
    {
        // Arrange
        var originalDocs = new List<string> { "doc1", "doc2" };

        // Act
        var result = _indexOperator.Operation(FixedIndex, new List<string>(), originalDocs);

        // Assert
        Assert.Equal(originalDocs, result);
    }
}