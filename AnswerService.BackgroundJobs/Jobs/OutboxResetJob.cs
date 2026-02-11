using AnswerService.Outbox.Interfaces.Service;
using Serilog;

namespace AnswerService.BackgroundJobs.Jobs;

public class OutboxResetJob(IOutboxResetService outboxResetService, ILogger logger)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var result = await outboxResetService.ResetOutboxMessagesAsync(cancellationToken);
        if (result.IsSuccess)
            logger.Information("Successfully reset outbox messages");
        else
            logger.Error("Failed to reset outbox messages: {message}", result.ErrorMessage);
    }
}