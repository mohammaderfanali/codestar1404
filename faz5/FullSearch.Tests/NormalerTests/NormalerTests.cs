using FluentAssertions;
using Xunit;

namespace SearchEngine;

public class NormalizerTests
{
    private readonly Normalizer _normalizer;

    public NormalizerTests()
    {
        _normalizer = new Normalizer();
    }
    
    [Theory]
    [InlineData("Hello World", "hello world")]                        
    [InlineData("Hello, World!", "hello world")]                       
    [InlineData("  leading & trailing spaces  ", "  leading  trailing spaces  ")] 
    [InlineData("TESTING 123 NUMBERS", "testing 123 numbers")] 
    [InlineData("!@#$%^&*()+_", "")]                                  
    [InlineData("This is a REGEX test.", "this is a regex test")]
    [InlineData("", "")]                                        
    [InlineData(null, null)]                                  
    public void Normalize_ShouldReturnCorrectString_ForVariousInputs(string input, string expectedResult)
    {
        
        // Act 
        var actualResult = _normalizer.Normalize(input);

        // Assert 
        actualResult.Should().Be(expectedResult);
    }
}