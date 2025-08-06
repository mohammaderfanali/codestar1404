using FluentAssertions;
using NSubstitute;
using Xunit;

namespace SearchEngine.Tests;

public class TokenizerTests
{
    private readonly INormalizer _mockNormalizer;
    private readonly Tokenizer _tokenizer;

    public TokenizerTests()
    {
        _mockNormalizer = Substitute.For<INormalizer>(); 
        _tokenizer = new Tokenizer(_mockNormalizer);
    }
    public static IEnumerable<object[]> TokenizationTestData()
    {
        yield return new object[]
        {
            "Hello, world. This-is_a?test!",
            "hello world this is a test",
            new List<string> { "hello", "world", "this", "is", "a", "test" }
        };
        
        yield return new object[]
        {
            "many,,,  separators--here_",
            "many separators here",
            new List<string> { "many", "separators", "here" }
        };
        
        yield return new object[]
        {
            "first line\nsecond line",
            "first line second line",
            new List<string> { "first", "line", "second", "line" }
        };
    }

    [Theory]
    [MemberData(nameof(TokenizationTestData))]
    public void Tokenize_ShouldCorrectlySplitNormalizedString(string originalInput, string normalizedString, List<string> expectedTokens)
    {
        // Arrange
        _mockNormalizer.Normalize(originalInput).Returns(normalizedString);

        // Act
        var actualTokens = _tokenizer.Tokenize(originalInput);

        // Assert
        actualTokens.Should().BeEquivalentTo(expectedTokens, options => options.WithStrictOrdering());
    }
}