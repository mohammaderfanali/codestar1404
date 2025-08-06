using FluentAssertions;
using NSubstitute;
using SearchEngine;
using System.Collections.Generic;
using Xunit;

namespace SearchEngine.Tests;

public class InvertedIndexBuilderTests
{
    private readonly ITokenizer _mockTokenizer;
    private readonly InvertedIndexBuilder _builder;

    public InvertedIndexBuilderTests()
    {
        _mockTokenizer = Substitute.For<ITokenizer>();
        _builder = new InvertedIndexBuilder();
    }

    [Fact]
    public void Build_ShouldCreateCorrectInvertedIndex_FromGivenFiles()
    {
        // --- Arrange ---

        var filesContent = new Dictionary<string, string>
        {
            { "1.txt", "this file for test : my name is mohammadReze Monemian" },
            { "2.txt", "hi this project FullSeaech that implement with inverted Index ...." },
            { "3.txt", "Multi-line\ncontent for\nthe tokenizer." }
        };

        // ۲. رفتار ماک Tokenizer را برای هر فایل تعریف می‌کنیم
        _mockTokenizer.Tokenize(filesContent["1.txt"])
            .Returns(new List<string> { "this", "file", "for", "test", "my", "name", "is", "mohammadreze", "monemian" });
        
        _mockTokenizer.Tokenize(filesContent["2.txt"])
            .Returns(new List<string> { "hi", "this", "project", "fullseaech", "that", "implement", "with", "inverted", "index" });
        
        _mockTokenizer.Tokenize(filesContent["3.txt"])
            .Returns(new List<string> { "multi", "line", "content", "for", "the", "tokenizer" });

        // ۳. نتیجه نهایی که انتظار داریم را می‌سازیم
        var expectedInvertedIndex = new Dictionary<string, Dictionary<string, List<int>>>
        {
            { "this", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 1 } }, { "2.txt", new List<int> { 2 } } } },
            { "file", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 2 } } } },
            { "for", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 3 } }, { "3.txt", new List<int> { 4 } } } },
            { "test", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 4 } } } },
            { "my", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 5 } } } },
            { "name", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 6 } } } },
            { "is", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 7 } } } },
            { "mohammadreze", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 8 } } } },
            { "monemian", new Dictionary<string, List<int>> { { "1.txt", new List<int> { 9 } } } },
            { "hi", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 1 } } } },
            { "project", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 3 } } } },
            { "fullseaech", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 4 } } } },
            { "that", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 5 } } } },
            { "implement", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 6 } } } },
            { "with", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 7 } } } },
            { "inverted", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 8 } } } },
            { "index", new Dictionary<string, List<int>> { { "2.txt", new List<int> { 9 } } } },
            { "multi", new Dictionary<string, List<int>> { { "3.txt", new List<int> { 1 } } } },
            { "line", new Dictionary<string, List<int>> { { "3.txt", new List<int> { 2 } } } },
            { "content", new Dictionary<string, List<int>> { { "3.txt", new List<int> { 3 } } } },
            { "the", new Dictionary<string, List<int>> { { "3.txt", new List<int> { 5 } } } },
            { "tokenizer", new Dictionary<string, List<int>> { { "3.txt", new List<int> { 6 } } } }
        };

        // --- Act ---
        var actualInvertedIndex = _builder.Build(filesContent, _mockTokenizer);

        // --- Assert ---
        actualInvertedIndex.Should().BeEquivalentTo(expectedInvertedIndex);
    }
}