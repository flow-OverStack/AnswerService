using AnswerService.Domain.Enums;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Outbox.Events;
using AnswerService.Outbox.Interfaces.Service;
using AnswerService.Outbox.Messages;
using AnswerService.Tests.FunctionalTests.Base.Exception.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class OutboxBackgroundServiceResilienceTests(OutboxProcessorFailureFunctionalTestWebAppFactory factory)
    : OutboxProcessorFailureFunctionalTest(factory)
{
    [Fact]
    public async Task ExecuteBackgroundJob_FirstTickThrows_StillProcessesOnALaterTick()
    {
        //Arrange
        const long authorId = 1;

        await using var scope = ServiceProvider.CreateAsyncScope();
        var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OutboxMessage>>();

        await outboxService.AddToOutboxAsync(new BaseEvent
        {
            EventId = Guid.NewGuid(),
            EventType = nameof(BaseEventType.EntityUpvoted),
            AuthorId = authorId
        });

        //Act
        //First tick (~15s) throws via the mocked processor; second tick (~30s) must still run and process
        //the message, proving the background service survives a transient failure instead of exiting for good.
        await Task.Delay(TimeSpan.FromSeconds(35));

        //Assert
        var outboxMessages = await outboxRepository.GetAll().AsNoTracking().ToListAsync();
        Assert.True(outboxMessages.All(x => x.ProcessedAt != null));
    }
}
