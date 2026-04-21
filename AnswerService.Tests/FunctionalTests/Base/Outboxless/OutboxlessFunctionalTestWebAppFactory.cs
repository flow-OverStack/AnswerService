using AnswerService.Outbox;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace AnswerService.Tests.FunctionalTests.Base.Outboxless;

public class OutboxlessFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.First(x =>
                x.ServiceType == typeof(IHostedService) &&
                x.ImplementationType == typeof(OutboxBackgroundService));

            services.Remove(descriptor);
        });
    }
}