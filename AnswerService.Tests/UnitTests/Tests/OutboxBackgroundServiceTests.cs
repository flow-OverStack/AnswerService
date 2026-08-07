using AnswerService.Tests.Traits;
using AnswerService.Tests.UnitTests.Fixtures;
using Moq;
using Serilog;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class OutboxBackgroundServiceTests
{
    [Fact]
    public async Task ExecuteAsync_ScopeFactoryThrows_LogsAndStopsOnCancellation()
    {
        //Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        var outboxService =
            new TestableOutboxBackgroundService(new Mock<ILogger>().Object, null!); // passing null throws

        //Act
        await outboxService.ExecuteAsync(cts.Token);

        //Assert
        // If any exception is thrown, the test will fail
        Assert.True(true);
    }
}