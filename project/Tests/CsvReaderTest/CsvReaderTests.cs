using project.csvReader;
using Xunit;

namespace Tests.CsvReaderTest;

public class CsvReaderTests
{
    [Fact]
    public void ReadCsvFile_ValidFile_ReturnsParsedData()
    {
        string tempFile = Path.GetTempFileName();
        File.WriteAllLines(tempFile, new[]
        {
            "name,age,city",
            "Ali,30,Tehran",
            "Sara,25,Isfahan"
        });

        var reader = new CsvReader();

        var result = reader.ReadCsvFile(tempFile);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(new[] { "name", "age", "city" }, result[0]);
        Assert.Equal(new[] { "Ali", "30", "Tehran" }, result[1]);
        Assert.Equal(new[] { "Sara", "25", "Isfahan" }, result[2]);

        File.Delete(tempFile);
    }

    [Fact]
    public void ReadCsvFile_FileNotFound_ReturnsNull()
    {
        string fakePath = "nonexistent.csv";
        var reader = new CsvReader();

        var result = reader.ReadCsvFile(fakePath);

        Assert.Null(result);
    }

    [Fact]
    public void ReadCsvFile_EmptyLines_IgnoresThem()
    {
        string tempFile = Path.GetTempFileName();
        File.WriteAllLines(tempFile, new[]
        {
            "name,age",
            "",
            "Ali,30",
            "   ",
            "Sara,25"
        });

        var reader = new CsvReader();

        var result = reader.ReadCsvFile(tempFile);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(new[] { "name", "age" }, result[0]);
        Assert.Equal(new[] { "Ali", "30" }, result[1]);
        Assert.Equal(new[] { "Sara", "25" }, result[2]);

        File.Delete(tempFile);
    }
}