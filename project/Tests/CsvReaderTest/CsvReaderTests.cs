using Microsoft.Extensions.Logging;
using NSubstitute;
using project.ReadCsv;
using Xunit;

namespace Tests.CsvReaderTest;

public class CsvReaderTests
{
    private readonly ILogger<CsvReader> _logger;
    private readonly CsvReader _reader;

    public CsvReaderTests()
    {
        _logger = Substitute.For<ILogger<CsvReader>>();
        _reader = new CsvReader(_logger);
    }

    [Fact]
    public void ReadCsvFile_ValidFile_ReturnsDataTableWithRowsAndColumns()
    {
        var tempFile = Path.GetTempFileName();
        File.WriteAllLines(tempFile, new[]
        {
            "Name,Age,City",
            "Ali,30,Tehran",
            "Sara,25,Shiraz"
        });

        var result = _reader.ReadCsvFile(tempFile);

        Assert.Equal("Name", result.Columns[0].ColumnName);
        Assert.Equal("Age", result.Columns[1].ColumnName);
        Assert.Equal("City", result.Columns[2].ColumnName);
        Assert.Equal(2, result.Rows.Count);
        Assert.Equal("Ali", result.Rows[0]["Name"]);
        Assert.Equal("Shiraz", result.Rows[1]["City"]);
        Assert.Equal(Path.GetFileNameWithoutExtension(tempFile), result.TableName);

        File.Delete(tempFile);
    }

    [Fact]
    public void ReadCsvFile_EmptyFile_ReturnsEmptyTableAndLogsWarning()
    {
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "");

        var result = _reader.ReadCsvFile(tempFile);

        Assert.Empty(result.Columns);
        Assert.Empty(result.Rows);

        _logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("is empty")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>()
        );

        File.Delete(tempFile);
    }

    [Fact]
    public void ReadCsvFile_RowWithWrongColumnCount_IsSkippedAndLogged()
    {
        var tempFile = Path.GetTempFileName();
        File.WriteAllLines(tempFile, new[]
        {
            "Name,Age",
            "Ali,30",
            "InvalidRowWithExtraColumn,Extra,Oops"
        });

        var result = _reader.ReadCsvFile(tempFile);

        Assert.Equal(1, result.Rows.Count);
        Assert.Equal("Ali", result.Rows[0]["Name"]);

        _logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Skipping line")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>()
        );

        File.Delete(tempFile);
    }

    [Fact]
    public void ReadCsvFile_FileNotFound_ThrowsAndLogsError()
    {
        var path = "nonexistent.csv";

        var ex = Assert.Throws<FileNotFoundException>(() => _reader.ReadCsvFile(path));

        _logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("was not found")),
            Arg.Any<FileNotFoundException>(),
            Arg.Any<Func<object, Exception, string>>()
        );
    }
}