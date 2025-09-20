using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using project.DataBase.DatabaseHealthChecker.Abstraction;
using project.Models.pluginoutput;
using project.Plugins.PluginClasses;
using Xunit;

namespace Tests.DatabasePluginTest
{
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
            var jsonString = File.ReadAllText("../../../DatabasePluginTest/test.json");
            var commandElement = JsonDocument.Parse(jsonString).RootElement;
            var emptyPastOutputs = new List<PluginOutput>();
            
            _dbChecker.IsConnectionValidAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));
            _dbChecker.TableHasDataAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));
            var result = await _sut.Makequery(commandElement, CancellationToken.None, emptyPastOutputs);
            
            Assert.NotNull(result);
            Assert.Contains("SELECT * FROM \"users_info\"", result.Query);
            Assert.Contains("Host=localhost", result.ConnectionString);
            await _dbChecker.Received(1).IsConnectionValidAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            await _dbChecker.Received(1)
                .TableHasDataAsync(Arg.Any<string>(), Arg.Is("users_info"), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task When_ConnectionIsValidAndTableIsEmpty_Expect_InvalidOperationExceptionToBeThrown()
        {
            var jsonString = File.ReadAllText("../../../DatabasePluginTest/test.json");
            var commandElement = JsonDocument.Parse(jsonString).RootElement;
            var emptyPastOutputs = new List<PluginOutput>();
            
            _dbChecker.IsConnectionValidAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));
            _dbChecker.TableHasDataAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));
            
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.Makequery(commandElement, CancellationToken.None, emptyPastOutputs));
        }

        [Fact]
        public async Task When_ConnectionIsInvalid_Expect_InvalidOperationExceptionToBeThrown()
        {
            var jsonString = File.ReadAllText("../../../DatabasePluginTest/test.json");
            var commandElement = JsonDocument.Parse(jsonString).RootElement;
            
            _dbChecker.IsConnectionValidAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));
            
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.Makequery(commandElement, CancellationToken.None, new List<PluginOutput>()));
        }

        [Fact]
        public async Task When_PastOutputsIsNotEmpty_Expect_ArgumentExceptionToBeThrown()
        {
            var jsonString = File.ReadAllText("../../../DatabasePluginTest/test.json");
            var commandElement = JsonDocument.Parse(jsonString).RootElement;
            var pastOutputs = new List<PluginOutput> { new("some query", "some connection") };
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.Makequery(commandElement, CancellationToken.None, pastOutputs));
        }

        [Fact]
        public async Task When_CommandIsInvalid_Expect_InvalidOperationExceptionToBeThrown()
        {
            var jsonString = File.ReadAllText("../../../DatabasePluginTest/invalidtest.json");
            var commandElement = JsonDocument.Parse(jsonString).RootElement;

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.Makequery(commandElement,CancellationToken.None,new List<PluginOutput>()));
        }

        [Fact]
        public async Task When_DbCheckerThrowsException_Expect_ExceptionToBeRethrown()
        {
            var jsonString = File.ReadAllText("../../../DatabasePluginTest/test.json");
            var commandElement = JsonDocument.Parse(jsonString).RootElement;
            var expectedException = new Exception("A database error occurred");

            _dbChecker.IsConnectionValidAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(expectedException);

            var actualException = await Assert.ThrowsAsync<Exception>(() =>
                _sut.Makequery(commandElement, CancellationToken.None, new List<PluginOutput>()));
            Assert.Equal(expectedException, actualException);
        }

        [Fact]
        public async Task When_MakeQueryIsCalledWithCanceledToken_Expect_OperationCanceledExceptionToBeThrown()
        {
            var jsonString = File.ReadAllText("../../../DatabasePluginTest/test.json");
            var commandElement = JsonDocument.Parse(jsonString).RootElement;

            var cts = new CancellationTokenSource();
            cts.Cancel();
            
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _sut.Makequery(commandElement,cts.Token, new List<PluginOutput>()));
        }
    }
}