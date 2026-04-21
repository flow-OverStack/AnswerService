using AnswerService.Domain.Enums;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Outbox.Events;
using AnswerService.Outbox.Interfaces.Service;
using AnswerService.Outbox.Messages;
using AnswerService.Tests.FunctionalTests.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests;

public class OutboxBackgroundServiceTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task ExecuteBackgroundJob_ShouldBe_Ok()
    {
        //Arrange
        const long userId = 1;

        await using var scope = ServiceProvider.CreateAsyncScope();
        var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OutboxMessage>>();

        await outboxService.AddToOutboxAsync(new BaseEvent
        {
            EventId = Guid.NewGuid(),
            EventType = nameof(BaseEventType.EntityUpvoted),
            AuthorId = userId
        });

        //Act
        await Task.Delay(TimeSpan.FromSeconds(20)); //Waiting for OutboxBackgroundService to execute the job

        //Assert
        var outboxMessages = await outboxRepository.GetAll().AsNoTracking().ToListAsync();
        Assert.True(outboxMessages.All(x => x.ProcessedAt != null));
    }
}