using AnswerService.Domain.Enums;

namespace AnswerService.Domain.Interfaces.Producer;

public interface IBaseEventProducer
{
    Task ProduceAsync(long authorId, long initiatorId, long answerId, BaseEventType eventType,
        CancellationToken cancellationToken = default);
}