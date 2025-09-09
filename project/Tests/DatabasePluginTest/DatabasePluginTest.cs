using System.Text.Json;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using project.DatabaseHealthChecker.Abstraction;
using project.Models.pluginoutput;
using project.Plugins.PluginClasses;
using Xunit;

namespace Tests.DatabasePluginTest;

public class DatabasePluginTests
{
    private readonly ILogger<DatabasePlugin> _logger;
    private readonly IDatabaseHealthChecker _dbChecker;
    private readonly DatabasePlugin _sut;

    public DatabasePluginTests()
    {
        _logger = Substitute.For<ILogger<DatabasePlugin>>();
        _dbChecker = Substitute.For<IDatabaseHealthChecker>();
        
        _sut = new DatabasePlugin(_logger, _dbChecker);
    }

    [Fact]
    public async Task When_ConnectionIsValidAndTableHasData_Expect_CorrectOutputToBeReturned()
    {
        // Arrange
        var jsonString = File.ReadAllText("../../../DatabasePluginTest/test.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;

        _dbChecker.IsConnectionValidAsync(Arg.Any<string>()).Returns(Task.FromResult(true));
        _dbChecker.TableHasDataAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(true));

        var result = await _sut.Makequery(commandElement);

        Assert.NotNull(result);
        Assert.Contains("SELECT * FROM \"users_info\"", result.Query);
        Assert.Contains("Host=localhost", result.ConnectionString);
        await _dbChecker.Received(1).IsConnectionValidAsync(Arg.Any<string>());
        await _dbChecker.Received(1).TableHasDataAsync(Arg.Any<string>(), Arg.Is("users_info"));
    }

    [Fact]
    public async Task When_ConnectionIsValidAndTableIsEmpty_Expect_WarningLogAndCorrectOutput()
    {
        var jsonString = File.ReadAllText("../../../DatabasePluginTest/test.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;
        
        _dbChecker.IsConnectionValidAsync(Arg.Any<string>()).Returns(Task.FromResult(true));
        _dbChecker.TableHasDataAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(false));

        var result = await _sut.Makequery(commandElement);
        
        Assert.NotNull(result);
        _logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString().Contains("is empty or does not exist")),
            null,
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task When_ConnectionIsInvalid_Expect_InvalidOperationExceptionToBeThrown()
    {
        var jsonString = File.ReadAllText("../../../DatabasePluginTest/test.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;

        _dbChecker.IsConnectionValidAsync(Arg.Any<string>()).Returns(Task.FromResult(false));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Makequery(commandElement));
    }

    [Fact]
    public async Task When_PastOutputsIsNotEmpty_Expect_ArgumentExceptionToBeThrown()
    {
        
        var jsonString = File.ReadAllText("../../../DatabasePluginTest/test.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;
        var pastOutputs = new List<PluginOutput> { new("some query", "some connection") };
        
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.Makequery(commandElement, pastOutputs));
    }

    [Fact]
    public async Task When_CommandIsInvalid_Expect_InvalidOperationExceptionToBeThrown()
    {
        
        var jsonString = File.ReadAllText("../../../DatabasePluginTest/invalidtest.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Makequery(commandElement));
    }
    
    [Fact]
    public async Task When_DbCheckerThrowsException_Expect_ExceptionToBeRethrown()
    {
        var jsonString = File.ReadAllText("../../../DatabasePluginTest/test.json");
        var commandElement = JsonDocument.Parse(jsonString).RootElement;
        var expectedException = new Exception("A database error occurred");

      
        _dbChecker.IsConnectionValidAsync(Arg.Any<string>()).ThrowsAsync(expectedException);

        var actualException = await Assert.ThrowsAsync<Exception>(() => _sut.Makequery(commandElement));
        Assert.Equal(expectedException, actualException); // Verify it's the same exception
    }
}