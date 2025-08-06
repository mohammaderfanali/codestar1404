using FluentAssertions;
using NSubstitute;
using SearchEngine;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SearchEngine.Tests;

public class NotQueryStrategyTests
{
    private readonly IQueryHandler _mockQueryHandler;
    private readonly IPhraseSearcher _mockSearcher;

    public NotQueryStrategyTests()
    {
        _mockQueryHandler = Substitute.For<IQueryHandler>();
        _mockSearcher = Substitute.For<IPhraseSearcher>();
    }

    [Fact]
    public void Execute_ShouldReturnAllDocsExceptExcludedOnes_WhenNotTermIsGiven()
    {
        // --- Arrange ---
        var fakeInvertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "any", new Dictionary<string, List<int>> { { "doc1", null }, { "doc2", null }, { "doc3", null } } }
        };
        var notStrategy = new NotQueryStrategy(_mockQueryHandler, _mockSearcher, fakeInvertedIndex);

        var fakeQueryClauses = new List<(string, char)> { ("term1", '-') };
        _mockQueryHandler.SplitCommandWithRegex(Arg.Any<string>()).Returns(fakeQueryClauses);

        _mockSearcher.Search(Arg.Any<Dictionary<string,Dictionary<string, List<int>>>>(), Arg.Is<List<string>>(l => l.SequenceEqual(new[] { "term1" })))
                     .Returns(new List<string> { "doc2" });

        // --- Act ---
        var result = notStrategy.Execute("-term1");

        // --- Assert ---
        result.Should().BeEquivalentTo(new[] { "doc1", "doc3" });
    }


    [Fact]
    public void Execute_ShouldExcludeUnionOfAllNotTerms_WhenMultipleNotTermsAreGiven()
    {
        // --- Arrange ---
        var fakeInvertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "any", new Dictionary<string, List<int>> { { "doc1", null }, { "doc2", null }, { "doc3", null } } }
        };
        var notStrategy = new NotQueryStrategy(_mockQueryHandler, _mockSearcher, fakeInvertedIndex);

        var fakeQueryClauses = new List<(string, char)> { ("term1", '-'), ("term2", '-') };
        _mockQueryHandler.SplitCommandWithRegex(Arg.Any<string>()).Returns(fakeQueryClauses);

        _mockSearcher.Search(Arg.Any<Dictionary<string,Dictionary<string, List<int>>>>(), Arg.Is<List<string>>(l => l.SequenceEqual(new[] { "term1" })))
                     .Returns(new List<string> { "doc1" });
        _mockSearcher.Search(Arg.Any<Dictionary<string,Dictionary<string, List<int>>>>(), Arg.Is<List<string>>(l => l.SequenceEqual(new[] { "term2" })))
                     .Returns(new List<string> { "doc3" });

        // --- Act ---
        var result = notStrategy.Execute("-term1 -term2");

        // --- Assert ---
        result.Should().ContainSingle().And.BeEquivalentTo(new[] { "doc2" });
    }


    [Fact]
    public void Execute_ShouldReturnAllDocs_WhenQueryHasNoNotTerms()
    {
        // --- Arrange ---
        var fakeInvertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "any", new Dictionary<string, List<int>> { { "doc1", null }, { "doc2", null } } }
        };
        var notStrategy = new NotQueryStrategy(_mockQueryHandler, _mockSearcher, fakeInvertedIndex);

        var fakeQueryClauses = new List<(string, char)> { ("term1", ' '), ("term2", '+') };
        _mockQueryHandler.SplitCommandWithRegex(Arg.Any<string>()).Returns(fakeQueryClauses);

        // --- Act ---
        var result = notStrategy.Execute("term1 +term2");

        // --- Assert ---
        var expectedDocs = new[] { "doc1", "doc2" };
        result.Should().BeEquivalentTo(expectedDocs);
    }
}