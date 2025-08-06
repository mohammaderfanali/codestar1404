using FluentAssertions;
using NSubstitute;
using SearchEngine;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SearchEngine.Tests;

public class AndQueryStrategyTests
{
    private readonly IQueryHandler _mockQueryHandler;
    private readonly IPhraseSearcher _mockSearcher;

    public AndQueryStrategyTests()
    {
        _mockQueryHandler = Substitute.For<IQueryHandler>();
        _mockSearcher = Substitute.For<IPhraseSearcher>();
    }

    [Fact]
    public void Execute_ShouldReturnIntersectionOfDocs_WhenMultipleAndTermsAreGiven()
    {

        // --- Arrange ---
        var fakeInvertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "any", new Dictionary<string, List<int>> { { "doc1", null }, { "doc2", null }, { "doc3", null } } }
        };
        var andStrategy = new AndQueryStrategy(_mockQueryHandler, _mockSearcher, fakeInvertedIndex);

        var fakeQueryClauses = new List<(string, char)> { ("term1", ' '), ("term2", ' ') };
        _mockQueryHandler.SplitCommandWithRegex(Arg.Any<string>()).Returns(fakeQueryClauses);

        _mockSearcher.Search(Arg.Any<Dictionary<string,Dictionary<string, List<int>>>>(), Arg.Is<List<string>>(l => l.SequenceEqual(new[] { "term1" })))
                     .Returns(new List<string> { "doc1", "doc2" });

        _mockSearcher.Search(Arg.Any<Dictionary<string,Dictionary<string, List<int>>>>(), Arg.Is<List<string>>(l => l.SequenceEqual(new[] { "term2" })))
                     .Returns(new List<string> { "doc2", "doc3" });

        // --- Act ---
        var result = andStrategy.Execute("term1 term2");

        // --- Assert ---
        result.Should().BeEquivalentTo(new[] { "doc2" });
    }

    [Fact]
    public void Execute_ShouldReturnEmptyList_WhenOneAndTermIsNotFound()
    {
   

        // --- Arrange ---
        var fakeInvertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "any", new Dictionary<string, List<int>> { { "doc1", null }, { "doc2", null } } }
        };
        var andStrategy = new AndQueryStrategy(_mockQueryHandler, _mockSearcher, fakeInvertedIndex);

        var fakeQueryClauses = new List<(string, char)> { ("found_term", ' '), ("missing_term", ' ') };
        _mockQueryHandler.SplitCommandWithRegex(Arg.Any<string>()).Returns(fakeQueryClauses);

        _mockSearcher.Search(Arg.Any<Dictionary<string,Dictionary<string, List<int>>>>(), Arg.Is<List<string>>(l => l.SequenceEqual(new[] { "found_term" })))
                     .Returns(new List<string> { "doc1", "doc2" });

        _mockSearcher.Search(Arg.Any<Dictionary<string,Dictionary<string, List<int>>>>(), Arg.Is<List<string>>(l => l.SequenceEqual(new[] { "missing_term" })))
                     .Returns(new List<string>()); 

        // --- Act ---
        var result = andStrategy.Execute("found_term missing_term");

        // --- Assert ---
        result.Should().BeEmpty();
    }
    
    [Fact]
    public void Execute_ShouldReturnAllDocs_WhenQueryHasNoAndTerms()
    {
  
        
        // --- Arrange ---
        var fakeInvertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "any", new Dictionary<string, List<int>> { { "doc1", null }, { "doc2", null }, { "doc3", null } } }
        };
        var andStrategy = new AndQueryStrategy(_mockQueryHandler, _mockSearcher, fakeInvertedIndex);

        var fakeQueryClauses = new List<(string, char)> { ("term1", '+'), ("term2", '-') };
        _mockQueryHandler.SplitCommandWithRegex(Arg.Any<string>()).Returns(fakeQueryClauses);
        
        // --- Act ---
        var result = andStrategy.Execute("+term1 -term2");

        // --- Assert ---
        var expectedDocs = new[] { "doc1", "doc2", "doc3" };
        result.Should().BeEquivalentTo(expectedDocs);
    }
}