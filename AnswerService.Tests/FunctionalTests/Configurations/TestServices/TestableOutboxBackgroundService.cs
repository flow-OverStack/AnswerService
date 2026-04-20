using AnswerService.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AnswerService.Tests.FunctionalTests.Configurations.TestServices;

internal class TestableOutboxBackgroundService(ILogger logger, IServiceScopeFactory scopeFactory)
    : OutboxBackgroundService(logger, scopeFactory)
{
    public new Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return base.ExecuteAsync(cancellationToken);
    }
}