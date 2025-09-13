using System.Text.Json;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using project.DataBase.CreateTableFromQuery.Abstraction;
using project.Models.pluginoutput;
using project.Plugins.PluginClasses;
using project.TransferTablefromQuery.Abstraction;
using Xunit;

public class OutputPluginTests
{
    private readonly ILogger<OutputPlugin> _logger;
    private readonly ITableCreator _tableCreator;
    private readonly IDataInserter _dataInserter;
    private readonly OutputPlugin _sut;
    private readonly  string uploadconnection;

    public OutputPluginTests()
    {
        _logger = Substitute.For<ILogger<OutputPlugin>>();
        _tableCreator = Substitute.For<ITableCreator>();
        _dataInserter = Substitute.For<IDataInserter>();
        uploadconnection = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=postgres;";

        _sut = new OutputPlugin(_logger, _tableCreator, _dataInserter);
    }

    [Fact]
    public async Task When_ValidInputsProvided_ShouldCallDependenciesAndSucceed()
    {
        var jsonString = await File.ReadAllTextAsync("../../../OutputPluginTest/test.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;
        var parentOutputs = new List<PluginOutput> { new("SELECT * FROM source", "source_connection") };


        var result = await _sut.Makequery(commandElement, CancellationToken.None, parentOutputs);

        Assert.Equal(parentOutputs[0], result);
        await _tableCreator.Received(1).CreateTableFromQueryAsync(
            "source_connection", "SELECT * FROM source",  uploadconnection , "final_customer_report",
            Arg.Any<CancellationToken>());
        await _dataInserter.Received(1).TransferDataAsync(
            "source_connection", "SELECT * FROM source",   uploadconnection, "final_customer_report",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_ParentOutputsAreEmpty_ShouldThrowArgumentException()
    {
        var jsonString = await File.ReadAllTextAsync("../../../OutputPluginTest/test.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;
        var emptyParentOutputs = new List<PluginOutput>();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.Makequery(commandElement, CancellationToken.None, emptyParentOutputs));
    }

    [Fact]
    public async Task When_DataInserterThrowsException_ShouldRethrowException()
    {
        var jsonString = await File.ReadAllTextAsync("../../../OutputPluginTest/test.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;
        var parentOutputs = new List<PluginOutput> { new("query", "connection") };
        var expectedException = new Exception("Data insertion failed");

        _tableCreator.CreateTableFromQueryAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _dataInserter.TransferDataAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .ThrowsAsync(expectedException);

        var actualException = await Assert.ThrowsAsync<Exception>(() =>
            _sut.Makequery(commandElement, CancellationToken.None,parentOutputs));
        Assert.Equal(expectedException, actualException);
        await _tableCreator.Received(1).CreateTableFromQueryAsync(Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_TokenIsCanceledInitially_ShouldThrowOperationCanceledException()
    {
        var jsonString = await File.ReadAllTextAsync("../../../OutputPluginTest/test.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;
        var parentOutputs = new List<PluginOutput> { new("query", "connection") };
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.Makequery(commandElement, cts.Token, parentOutputs));
    }
}