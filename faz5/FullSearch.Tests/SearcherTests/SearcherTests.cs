using FluentAssertions;
using SearchEngine;
using System.Collections.Generic;
using Xunit;

namespace SearchEngine.Tests;

public class SearcherIntegrationTests
{
    private readonly Dictionary<string, Dictionary<string, List<int>>> _testInvertedIndex;
    private readonly IQueryHandler _queryHandler;
    private readonly IPhraseSearcher _phraseSearcher;

    public SearcherIntegrationTests()
    {
        _queryHandler = new QueryHandler();
        _phraseSearcher = new PhraseSearcher();
        
        _testInvertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "this", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 1 } }, { "2.txt", new List<int> { 2 } } } },
            { "file", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 2 } } } },
            { "for", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 3 } }, { "3.txt", new List<int> { 4 } } } }, // "file for" در doc1
            { "project", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 3 } } } },
            { "inverted", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 8 } } } },
            { "index", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 9 } } } }, // "inverted index" در doc2
            { "content", new Dictionary<string, List<int>> { { "3.txt", new List<int> { 3 } } } },
            { "hi", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 1 } } } }
        };
    }

    public static IEnumerable<object[]> FullSearchQueries()
    {
        yield return new object[] { "project index", new List<string> { "2.txt" } };
        yield return new object[] { "+file +content", new List<string> { "1.txt", "3.txt" } };
        yield return new object[] { "-project", new List<string> { "1.txt", "3.txt" } };
        yield return new object[] { "this +file", new List<string> { "1.txt" } };
        yield return new object[] { "for -content", new List<string> { "1.txt" } };
        yield return new object[] { "+file +project -index", new List<string> { "1.txt" } };
        yield return new object[] { "this +hi -file", new List<string> { "2.txt" } };
        yield return new object[] { "this -this", new List<string>() };
        yield return new object[] { "\"inverted index\"", new List<string> { "2.txt" } };
        yield return new object[] { "\"this projects\"", new List<string>() }; 
        yield return new object[] { "+\"inverted index\" +file", new List<string> { "1.txt", "2.txt" } };
        yield return new object[] { "for \"file for\"", new List<string> { "1.txt" } };
        yield return new object[] { "-\"inverted index\"", new List<string> { "1.txt", "3.txt" } };
    }

    [Theory]
    [MemberData(nameof(FullSearchQueries))]
    public void Search_WithVariousQueries_ShouldReturnCorrectlyFilteredDocuments(string query, List<string> expectedDocs)
    {
        // Arrange
        var searcher = new Searcher(_testInvertedIndex);
        
        searcher.addfilter(new AndQueryStrategy(_queryHandler, _phraseSearcher, _testInvertedIndex));
        searcher.addfilter(new OrQueryStrategy(_queryHandler, _phraseSearcher, _testInvertedIndex));
        searcher.addfilter(new NotQueryStrategy(_queryHandler, _phraseSearcher, _testInvertedIndex));

        // Act
        var result = searcher.Search(query);

        // Assert
        result.Should().BeEquivalentTo(expectedDocs);
    }
}