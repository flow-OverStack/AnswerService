using MassTransit;
using MassTransit.Util;

namespace AnswerService.Tests.FunctionalTests.Configurations.TestServices;

internal class TestTopicProducer<T> : ITopicProducer<T> where T : class
{
    public ConnectHandle ConnectSendObserver(ISendObserver observer)
    {
        return new EmptyConnectHandle();
    }

    public Task Produce(T message, CancellationToken cancellationToken = new())
    {
        return Task.CompletedTask;
    }

    public Task Produce(T message, IPipe<KafkaSendContext<T>> pipe, CancellationToken cancellationToken = new())
    {
        return Task.CompletedTask;
    }

    public Task Produce(object values, CancellationToken cancellationToken = new())
    {
        return Task.CompletedTask;
    }

    public Task Produce(object values, IPipe<KafkaSendContext<T>> pipe, CancellationToken cancellationToken = new())
    {
        return Task.CompletedTask;
    }
}