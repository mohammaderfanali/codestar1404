using System.Data;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using project.DataBase.DataTableUpploader.Abstraction;
using project.Plugins.PluginClasses;
using project.Plugins.Pluginmodels;
using project.ReadCsv.Abstraction;
using Xunit;

namespace Tests.CsvPluginTest;

public class CsvPluginTests
{
    [Fact]
    public async Task Makequery_ValidCommand_UploadsDataAndReturnsQuery()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CsvPlugin>>();
        var csvReader = Substitute.For<ICsvReader>();
        var uploader = Substitute.For<IDataTableUplouder>();
        var config = Substitute.For<IConfiguration>();

        var fakeConnectionString = "Host=localhost;Database=testdb;Username=test;Password=test";
        var fakeFilePath = "sample.csv";

        // simulate path.uploadconnection
        var path = new { uploadconnection = fakeConnectionString };
        var plugin = new CsvPlugin(logger, csvReader, uploader, config);
        typeof(CsvPlugin).GetField("_connectionString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(plugin, fakeConnectionString);

        var fakeTable = new DataTable("sample");
        fakeTable.Columns.Add("Name");
        fakeTable.Columns.Add("Age");
        fakeTable.Rows.Add("Ali", "30");

        csvReader.ReadCsvFile(fakeFilePath).Returns(fakeTable);

        var commandModel = new CsvReaderModel { Filepath = fakeFilePath };
        var json = JsonSerializer.Serialize(commandModel);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        // Act
        var result = await plugin.Makequery(jsonElement, CancellationToken.None);

        // Assert
        await uploader.Received().UploadDataAsync(fakeConnectionString, fakeTable, CancellationToken.None);
        Assert.Equal("SELECT * FROM \"sample\"", result.Query);
        Assert.Equal(fakeConnectionString, result.ConnectionString);
    }
}