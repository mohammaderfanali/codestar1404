using Xunit;
using Microsoft.Extensions.Logging;
using project.ReadCsv;
using System.IO;

namespace Tests.CsvReaderTest
{
    public class CsvReaderTests
    {
        private readonly CsvReader _reader;
        private readonly string _tempFile;

        public CsvReaderTests()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<CsvReader>();
            _reader = new CsvReader(logger);
            _tempFile = Path.GetTempFileName();
        }

        [Fact]
        public void ReadCsvFile_WithValidFile_ReturnsDataWithoutHeader()
        {
            File.WriteAllLines(_tempFile, new[]
            {
                "name,age,city",
                "Ali,30,Tehran",
                "Sara,25,Isfahan"
            });

            var result = _reader.ReadCsvFile(_tempFile);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(new[] { "Ali", "30", "Tehran" }, result[0]);
            Assert.Equal(new[] { "Sara", "25", "Isfahan" }, result[1]);
            
            File.Delete(_tempFile);
        }

        [Fact]
        public void ReadCsvFile_WhenFileIsNotFound_ReturnsEmptyList()
        {
            string nonExistentFile = "this_file_does_not_exist.csv";

            var result = _reader.ReadCsvFile(nonExistentFile);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ReadCsvFile_WithEmptyLines_IgnoresThemAndReturnsData()
        {
            File.WriteAllLines(_tempFile, new[]
            {
                "name,age",
                "",
                "Ali,30",
                "   ",
                "Sara,25"
            });
            
            var result = _reader.ReadCsvFile(_tempFile);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(new[] { "Ali", "30" }, result[0]);
            Assert.Equal(new[] { "Sara", "25" }, result[1]);
            
            File.Delete(_tempFile);
        }
    }
}

