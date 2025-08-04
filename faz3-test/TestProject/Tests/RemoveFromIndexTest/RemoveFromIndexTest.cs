using System;
using System.Collections.Generic;
using Xunit;
using faz3;

public class RemoveFromIndexTests
{
    // Fixed inverted index for all tests
    private static readonly Dictionary<string, List<string>> FixedIndex = new()
    {
        { "apple", new List<string> { "doc1", "doc2", "doc3" } },
        { "banana", new List<string> { "doc1", "doc3", "doc5" } },
        { "cherry", new List<string> { "doc2", "doc4" } },
        { "date", new List<string> { "doc5", "doc6" } }
    };

    private readonly RemoveFromIndex _indexOperator = new();

    [Theory]
    [InlineData(
        new string[] { },              // excluded words
        new string[] { "doc1", "doc2" }, // input docs
        new string[] { "doc1", "doc2" }, // expected
        "Case 1: Empty excluded words - returns original docs"
    )]
    [InlineData(
        new[] { "apple" },             // excluded words
        new string[] { "doc1", "doc2", "doc3", "doc4" }, // input docs
        new string[] { "doc4" },       // expected
        "Case 2: Single excluded word - removes matching docs"
    )]
    [InlineData(
        new[] { "banana", "cherry" },  // excluded words
        new string[] { "doc1", "doc2", "doc3", "doc4", "doc5" }, // input docs
        new string[] {  },       // expected
        "Case 3: Multiple excluded words - removes union of matches"
    )]
    [InlineData(
        new[] { "date" },              // excluded words
        new string[] { "doc1", "doc5", "doc6" }, // input docs
        new string[] { "doc1" },       // expected
        "Case 4: Excludes non-overlapping docs"
    )]
    [InlineData(
        new[] { "missing" },           // excluded words
        new string[] { "doc1", "doc2" }, // input docs
        new string[] { "doc1", "doc2" }, // expected
        "Case 5: Non-existent excluded word - no change"
    )]
    [InlineData(
        new[] { "apple", "missing" },  // excluded words
        new string[] { "doc1", "doc2", "doc3", "doc4" }, // input docs
        new string[] { "doc4" },       // expected
        "Case 6: Mixed existent/non-existent words - only removes existing"
    )]
    [InlineData(
        new[] { "apple", "banana" },   // excluded words
        new string[] { "doc1", "doc3" }, // input docs
        new string[] { },              // expected
        "Case 7: All docs excluded - returns empty list"
    )]
    [InlineData(
        new[] { "apple" },             // excluded words
        new string[] { "doc1", "doc1", "doc2", "doc4" }, // input docs
        new string[] { "doc4" },       // expected
        "Case 8: Handles duplicate input docs"
    )]
    public void Operation_TheoryTests(
        string[] excluded,
        string[] docs,
        string[] expected,
        string testCaseDescription)
    {
        // Act
        var result = _indexOperator.Operation(
            FixedIndex,
            new List<string>(excluded),
            new List<string>(docs));

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Operation_DoesNotModifyOriginalInput()
    {
        // Arrange
        var originalDocs = new List<string> { "doc1", "doc2" };
        var copyOfOriginal = new List<string>(originalDocs);

        // Act
        _indexOperator.Operation(FixedIndex, new List<string> { "apple" }, originalDocs);

        // Assert
        Assert.Equal(copyOfOriginal.OrderBy(x=>x), originalDocs.OrderBy(x=>x));
    }
}