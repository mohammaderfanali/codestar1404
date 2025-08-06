using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SearchEngine.Tests;

public class OrQueryStrategyTests
{
    private readonly IQueryHandler _mockQueryHandler;
    private readonly IPhraseSearcher _mockSearcher;
    private readonly OrQueryStrategy _orStrategy;

    public OrQueryStrategyTests()
    {
        _mockQueryHandler = Substitute.For<IQueryHandler>();
        _mockSearcher = Substitute.For<IPhraseSearcher>();
        var fakeInvertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "any_word", new Dictionary<string, List<int>> 
                {
                    { "doc1", new List<int> { 1 } },
                    { "doc2", new List<int> { 1 } },
                    { "doc3", new List<int> { 1 } }
                } 
            }
        };
        _orStrategy = new OrQueryStrategy(_mockQueryHandler, _mockSearcher, fakeInvertedIndex);
    }

    [Fact]
    public void Execute_ShouldReturnUnionOfDocs_WhenMultipleOrTermsAreGiven()
    {
        var fakeQueryClauses = new List<(string term, char prefix)>
        {
            ("term1", '+'),
            ("term2", '+')
        };
        _mockQueryHandler.SplitCommandWithRegex(Arg.Any<string>()).Returns(fakeQueryClauses);
        _mockSearcher.Search(Arg.Any<Dictionary<string, Dictionary<string, List<int>>>>(), 
                             Arg.Is<List<string>>(list => list.SequenceEqual(new[] { "term1" })))
                     .Returns(new List<string> { "doc1", "doc2" });
        _mockSearcher.Search(Arg.Any<Dictionary<string, Dictionary<string, List<int>>>>(), 
                             Arg.Is<List<string>>(list => list.SequenceEqual(new[] { "term2" })))
                     .Returns(new List<string> { "doc2", "doc3" });

        
        var result = _orStrategy.Execute("any query");
        var expectedDocs = new[] { "doc1", "doc2", "doc3" };
        result.Should().BeEquivalentTo(expectedDocs);
    }
    
    [Fact]
    public void Execute_ShouldReturnAllDocs_WhenQueryHasNoOptionalTerms()
    {
        var invertedIndexWithDocs = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "any_word", new Dictionary<string, List<int>> 
                {
                    { "doc1", new List<int> { 1 } },
                    { "doc2", new List<int> { 1 } }
                } 
            }
        };
        var orStrategyWithDocs = new OrQueryStrategy(_mockQueryHandler, _mockSearcher, invertedIndexWithDocs);
        var fakeQueryClauses = new List<(string term, char prefix)>
        {
            ("term1", ' '),
            ("term2", ' ')
        };
        _mockQueryHandler.SplitCommandWithRegex(Arg.Any<string>()).Returns(fakeQueryClauses);

        
        var result = orStrategyWithDocs.Execute("a query without plus");

      
        var expectedDocs = new[] { "doc1", "doc2" };
        result.Should().BeEquivalentTo(expectedDocs);
    }
    
    [Fact]
    public void Execute_ShouldReturnDocsForFoundTerm_WhenOtherOptionalTermIsNotFound()
    {
        var fakeQueryClauses = new List<(string term, char prefix)>
        {
            ("found_term", '+'),
            ("not_found_term", '+')
        };
        _mockQueryHandler.SplitCommandWithRegex(Arg.Any<string>()).Returns(fakeQueryClauses);
        _mockSearcher.Search(Arg.Any<Dictionary<string, Dictionary<string, List<int>>>>(),
                Arg.Is<List<string>>(l => l.SequenceEqual(new[] { "found_term" })))
            .Returns(new List<string> { "doc1", "doc3" });
        _mockSearcher.Search(Arg.Any<Dictionary<string, Dictionary<string, List<int>>>>(),
                Arg.Is<List<string>>(l => l.SequenceEqual(new[] { "not_found_term" })))
            .Returns(new List<string>()); 

        var result = _orStrategy.Execute("+found_term +not_found_term");

        
        var expectedDocs = new[] { "doc1", "doc3" };
        result.Should().BeEquivalentTo(expectedDocs);
    }
}