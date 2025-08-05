using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using file;
using Tests.FileTest; // Make sure this namespace matches your actual code

public class FileReaderTests
{
    [Fact]
    public void ReadFiles_ReturnsCorrectFileContents()
    {
        // Arrange


        var expected = new Dictionary<string, string>
        {
            { "1", "i am Soroosh here" },
            { "2", "i am Soroosh" },
            { "3", "i am Soroosh" }
        };

        var reader = new FileReader();

        // Act
        Dictionary<string, string> result = reader.ReadFiles(Files.pathfile);

        // Assert
        Assert.True(expected.OrderBy(kvp => kvp.Key)
            .SequenceEqual(result.OrderBy(kvp => kvp.Key)));


    }
}