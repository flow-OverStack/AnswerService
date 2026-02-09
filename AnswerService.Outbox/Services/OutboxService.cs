using AnswerService.Outbox.Interfaces.Repository;
using AnswerService.Outbox.Interfaces.Service;
using AnswerService.Outbox.Messages;
using Newtonsoft.Json;

namespace AnswerService.Outbox.Services;

public class OutboxService(IOutboxRepository outboxRepository) : IOutboxService
{
    public Task AddToOutboxAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var outboxMessage = new OutboxMessage
        {
            Type = typeof(T).FullName ?? typeof(T).Name,
            Content = JsonConvert.SerializeObject(message)
        };

        return outboxRepository.AddAsync(outboxMessage, cancellationToken);
    }
}