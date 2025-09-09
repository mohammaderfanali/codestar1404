using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using project.csvReader.Abstraction;
using project.DataBaseUpploader.Abstraction;
using project.Models.pluginoutput;
using project.Plugins.PluginClasses;
using project.Plugins.Pluginmodels;
using Xunit;


public static class path
{
    public static string uploadconnection;
}

public class CsvPluginTests
{
    private readonly ILogger<CsvPlugin> _logger;
    private readonly ICsvReader _csvReader;
    private readonly IDataBaseUploader _dataBaseUploader;
    private readonly IConfiguration _configuration;
    private CsvPlugin _sut; // System Under Test

    public CsvPluginTests()
    {
        _logger = Substitute.For<ILogger<CsvPlugin>>();
        _csvReader = Substitute.For<ICsvReader>();
        _dataBaseUploader = Substitute.For<IDataBaseUploader>();
        _configuration = Substitute.For<IConfiguration>();
        
        path.uploadconnection = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=postgres;";

        _sut = new CsvPlugin(_logger, _csvReader, _dataBaseUploader, _configuration);
    }

    [Fact]
    public async Task When_MakeQueryIsCalledWithValidCommand_Expect_CorrectOutputToBeReturned()
    {
        var jsonString = File.ReadAllText("../../../CsvPluginTest/test.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;
        
        var filePath ="C:\\Users\\soroo\\Desktop\\users_contact.csv";
        var tableName = "users_contact";
        var expectedQuery = $"SELECT * FROM \"{tableName}\"";
        var expectedConnectionString = path.uploadconnection;
        var headers = new[] { "Id", "Name" };

        _csvReader.ReadCsvFile(filePath).Returns(new List<string[]>());
        _csvReader.GetColumnHeaders(filePath).Returns(headers);
        
        var result = await _sut.Makequery(commandElement);

        Assert.NotNull(result);
        Assert.Equal(expectedQuery, result.Query);
        Assert.Equal(expectedConnectionString, result.ConnectionString);

        await _dataBaseUploader.Received(1).UploadDataAsync(
            expectedConnectionString,
            tableName,
            headers, 
            Arg.Any<List<string[]>>());
    }

    [Fact]
    public async Task When_MakeQueryIsCalledWithNonEmptyHistory_Expect_ArgumentExceptionToBeThrown()
    {
        var jsonString = File.ReadAllText("../../../CsvPluginTest/test.json"); 
        var commandElement = JsonDocument.Parse(jsonString).RootElement;
        var pastOutputs = new List<PluginOutput> { new("some query", "some connection") };
        
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.Makequery(commandElement, pastOutputs));
    }

    [Fact]
    public async Task When_CommandFilePathIsMissing_Expect_InvalidOperationExceptionToBeThrown()
    {
        var jsonString = File.ReadAllText("../../../CsvPluginTest/invalidtest.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Makequery(commandElement));
    }

    [Fact]
    public async Task When_UploaderThrowsException_Expect_ExceptionToBeRethrown()
    {
        var jsonString = File.ReadAllText("../../../CsvPluginTest/test.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;
        var filePath = "C:\\data\\sample.csv";
        var expectedException = new IOException("Database is unavailable");

        _csvReader.ReadCsvFile(filePath).Returns(new List<string[]>());
        _csvReader.GetColumnHeaders(filePath).Returns(Array.Empty<string>());
        
        _dataBaseUploader.UploadDataAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string[]>(), Arg.Any<List<string[]>>())
                         .ThrowsAsync(expectedException);
        
        var actualException = await Assert.ThrowsAsync<IOException>(() => _sut.Makequery(commandElement));
        Assert.Equal(expectedException, actualException);
    }
}