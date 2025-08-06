using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace SearchEngine;
public class PhraseSearcherTests
{
    private readonly PhraseSearcher _phraseSearcher;

    public PhraseSearcherTests()
    {
        _phraseSearcher = new PhraseSearcher();
    }

 
    [Fact] 
    public void Should_ReturnSingleDoc_When_PhraseExistsInOneDoc()
    {   
        //Arrange
        var invertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "hello", new Dictionary<string, List<int>> { { "doc1", new List<int> { 5 } } } },
            { "world", new Dictionary<string, List<int>> { { "doc1", new List<int> { 6 } } } }
        };
        var phrase = new List<string> { "hello", "world" };
         
        //Act
        var result = _phraseSearcher.Search(invertedIndex, phrase);
         
        //Assert
        result.Should().NotBeNull()               
            .And.ContainSingle()               
            .Which.Should().Be("doc1");
    }

    [Fact]
    public void Should_ReturnEmpty_When_PhraseNotExistsInAnyDoc()
    {
        // Arrange
        var invertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "hello", new Dictionary<string, List<int>> { { "doc1", new List<int> { 5 } } } },
            { "world", new Dictionary<string, List<int>> { { "doc1", new List<int> { 10 } } } } 
        };
        var phrase = new List<string> { "hello", "world" };

        // Act
        var result = _phraseSearcher.Search(invertedIndex, phrase);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();;
    }
    

    [Fact]
    public void Should_ReturnEmpty_When_SomeOfWordNotExistInInvertedIndex()
    {
        // Arrange
        var invertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "hello", new Dictionary<string, List<int>> { { "doc1", new List<int> { 5 } } } }
        };
        var phrase = new List<string> { "hello", "world" };

        // Act
        var result = _phraseSearcher.Search(invertedIndex, phrase);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Should_ReturnSomeDoc_When_PhraseExistsInSomeDoc()
    {
        // Arrange
        var invertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "search", new Dictionary<string, List<int>> 
                { 
                    { "doc1", new List<int> { 1 } },
                    { "doc2", new List<int> { 8 } },
                    { "doc3", new List<int> { 5 } }
                } 
            },
            { "engine", new Dictionary<string, List<int>>
                {
                    { "doc1", new List<int> { 2 } },
                    { "doc2", new List<int> { 15 } },
                    { "doc3", new List<int> { 6 } }
                }
            }
        };
        var phrase = new List<string> { "search", "engine" };

        // Act
        var result = _phraseSearcher.Search(invertedIndex, phrase);

        // Assert
        result.Should().HaveCount(2)
            .And.Contain(new[] { "doc1", "doc3" });
     
    }
    
    [Fact]
    public void Should_ReturnDoc_When_PhraseWithSomeWordExistsInDoc()
    {
        // Arrange
        var invertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "search", new Dictionary<string, List<int>> 
                { 
                    { "doc1", new List<int> { 1 } },
                    { "doc2", new List<int> { 8 } },
                    { "doc3", new List<int> { 5 } }
                } 
            },
            { "engine", new Dictionary<string, List<int>>
                {
                    { "doc1", new List<int> { 2 } },
                    { "doc2", new List<int> { 15 } },
                    { "doc3", new List<int> { 6 } }
                }
            },
            { "google", new Dictionary<string, List<int>>
            {
                { "doc1", new List<int> { 3 } },
                { "doc2", new List<int> { 16 } },
                { "doc3", new List<int> { 9} }
            }
        }
        };
        var phrase = new List<string> { "search", "engine" , "google" };

        // Act
        var result = _phraseSearcher.Search(invertedIndex, phrase);

        // Assert
        result.Should().HaveCount(1)
            .And.Contain(new[] { "doc1"});
     
    }
    [Fact]
    public void Should_ReturnEmpty_When_PhraseIsEmpty()
    {
        // Arrange
        var invertedIndex = new Dictionary<string, Dictionary<string, List<int>>>(); // ایندکس مهم نیست
        var phrase = new List<string>();

        // Act
        var result = _phraseSearcher.Search(invertedIndex, phrase);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }
}