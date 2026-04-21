using AnswerService.Tests.UnitTests.Configurations;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

public class OutboxBackgroundServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task ExecuteBackgroundJob_ShouldBe_NoException()
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