
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using faz3;
using Xunit;
using file;


public class TokenizeTest
{
    [Theory]
    [InlineData("i am:soroosh", new[] { "i", "am", "soroosh" })]
    [InlineData("hello,soroosh", new[] { "hello", "soroosh" })]
    [InlineData("welcome?ali", new[] { "welcome", "ali" })]
    [InlineData("hi\nmy name is ,,,mohammad,my age is:00-18", new[]
    {
        "hi", "my","name","is","mohammad","my" , "age" , "is","00","18"
    })]
    public void TokenTest(string content, string[] expected)
    {
        // Arrange - Assuming you have a tokenizer class
        var tokenizer = new Tokenizer(); // Replace with your actual tokenizer
        
        // Act
        List<string> result = tokenizer.Tokenner(content); // Replace with your actual tokenize method
        
        // Assert
        Assert.Equal(expected, result);
    }
}