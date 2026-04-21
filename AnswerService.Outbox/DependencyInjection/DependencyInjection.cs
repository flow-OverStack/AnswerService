using AnswerService.Outbox.Events;
using AnswerService.Outbox.Interfaces.Repository;
using AnswerService.Outbox.Interfaces.Service;
using AnswerService.Outbox.Interfaces.TopicProducer;
using AnswerService.Outbox.Repositories;
using AnswerService.Outbox.Services;
using AnswerService.Outbox.TopicProducers;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.Outbox.DependencyInjection;

public static class DependencyInjection
{
    public static void AddOutbox(this IServiceCollection services)
    {
        services.InitServices();
        services.InitBackgroundServices();
        services.InitTopicProducers();
    }

    private static void InitBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxBackgroundService>();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
        services.AddScoped<IOutboxResetService, OutboxResetService>();
    }

    private static void InitTopicProducers(this IServiceCollection services)
    {
        services.AddSingleton<ITopicProducerResolver, TopicProducerResolver>();
        services.AddScoped<ITopicProducer, TopicProducer<BaseEvent>>();
    }
}