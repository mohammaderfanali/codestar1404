using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using faz3;

public class AddRequiredWordsToIndexTests
{
    // Fixed inverted index for all tests
    private static readonly Dictionary<string, List<string>> FixedIndex = new()
    {
        { "apple", new List<string> { "doc1", "doc2", "doc3" } },
        { "banana", new List<string> { "doc1", "doc3" } },
        { "cherry", new List<string> { "doc2", "doc4" } }
    };

    private readonly AddRequiredWordsToIndex _indexOperator = new();

    [Theory]
    [InlineData(
        new string[] { }, // required words
        new string[] { "doc1" }, // input docs
        new string[] { "doc1" }, // expected (empty)
        "T1"
    )]
    [InlineData(
        new[] { "missing" }, // required words
        new[] { "doc1", "doc2" }, // input docs
        new string[] { }, // expected (empty)
        "Non-existent word"
    )]
    [InlineData(
        new[] { "apple", "banana" }, // required words
        new[] { "doc1", "doc3" }, // input docs
        new[] { "doc1", "doc3" }, // expected result
        "Exact match intersection"
    )]
    [InlineData(
        new[] { "apple", "banana" }, // required words
        new[] { "doc5" }, // input docs
        new string[] { }, // expected (empty)
        "No overlap with input docs"
    )]
    [InlineData(
        new[] { "apple", "cherry" }, // required words
        new[] { "doc2" }, // input docs
        new[] { "doc2" }, // expected result
        "Partial intersection"
    )]
    public void Operation_ReturnsCorrectDocuments(
        string[] required,
        string[] docs,
        string[] expected,
        string testName)
    {
        // Act
        var result = _indexOperator.Operation(
            FixedIndex,
            new List<string>(required),
            new List<string>(docs));

        // Assert
        Assert.Equal(expected, result);
    }
}
