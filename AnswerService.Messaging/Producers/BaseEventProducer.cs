using AnswerService.Domain.Entities;
using AnswerService.Domain.Enums;
using AnswerService.Domain.Interfaces.Producer;
using AnswerService.Outbox.Events;
using AnswerService.Outbox.Interfaces.Service;

namespace AnswerService.Messaging.Producers;

public class BaseEventProducer(IOutboxService outboxService) : IBaseEventProducer
{
    public Task ProduceAsync(long authorId, long initiatorId, long answerId, BaseEventType eventType,
        CancellationToken cancellationToken = default)
    {
        var baseEvent = new BaseEvent
        {
            EventId = Guid.NewGuid(),
            AuthorId = authorId,
            InitiatorId = initiatorId,
            EntityId = answerId,
            EntityType = nameof(Answer),
            EventType = eventType.ToString()
        };

        return outboxService.AddToOutboxAsync(baseEvent, cancellationToken);
    }
}