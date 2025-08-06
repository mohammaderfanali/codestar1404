using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace SearchEngine.Tests;

public class FileReaderTests
{
    private readonly FileReader _fileReader;

    public FileReaderTests()
    {
        _fileReader = new FileReader();
        File.WriteAllText(Path.Combine(@"..\..\..\FileForTest","3.txt"), "Multi-line\ncontent for\nthe tokenizer.");
    }

    [Fact]
    public void ReadFiles_ShouldReturnContentOfAllFiles_FromSpecificDirectory()
    {
        // --- Arrange ---
        
        string testDirectoryPath = @"..\..\..\FileForTest";

        // --- Act ---
        var result = _fileReader.ReadFiles(testDirectoryPath);

        // --- Assert ---
        result.Should().NotBeNull();
        result.Should().HaveCount(3); 

        result.Should().ContainKey("1.txt")
            .WhoseValue.Should().Be("this file for test : my name is mohammadReze Monemian\r\n");
        
        result.Should().ContainKey("2.txt")
            .WhoseValue.Should().Be("hi this project FullSeaech that implement with inverted Index ....");
        
        result.Should().ContainKey("3.txt")
            .WhoseValue.Should().Be("Multi-line\ncontent for\nthe tokenizer.");
    }
}