using System.Collections.Generic;
using Xunit;
using NSubstitute;
using faz3;
using InvertedIndex;

public class AddToInvertedIndexTests
{
    [Fact]
    public void MakeInvertedindex_AddsWordsToIndex_Correctly()
    {
        // Arrange
        var files = new Dictionary<string, string>
        {
            { "doc1", "Hello world" },
            { "doc2", "Hello Soroosh" }
        };
        
        var expected = new Dictionary<string, List<string>>
        {
            { "hello", new List<string> { "doc1", "doc2" } },
            { "world", new List<string> { "doc1" } },
            { "soroosh", new List<string> { "doc2" } }
        };
        
        var tokenizer = Substitute.For<ITokenizer>();
        tokenizer.Tokenner("Hello world").Returns(new List<string> { "hello", "world" });
        tokenizer.Tokenner("Hello Soroosh").Returns(new List<string> { "hello", "soroosh" });

        var indexer = new AddToInvertedIndex();

        // Act
        indexer.MakeInvertedindex(files, tokenizer);

        // Assert
        var index = indexer.InvertedIndex;

        Assert.Equal(
            expected.OrderBy(kv => kv.Key),
            index.OrderBy(kv => kv.Key)
        );

    }
}