using AnswerService.Tests.UnitTests.Configurations;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class OutboxBackgroundServiceTests
{
    [Fact]
    public async Task ExecuteAsync_NullServiceScopeFactory_SwallowsException()
    {
        //Arrange
        var outboxService =
            new TestableOutboxBackgroundService(LoggerConfiguration.GetLogger(), null!); // passing null for exception

        //Act
        await outboxService.ExecuteAsync();

        //Assert
        // If any exception is thrown, the test will fail
        Assert.True(true);
    }
}