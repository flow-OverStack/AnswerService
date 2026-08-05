using AnswerService.Domain.Enums;
using AnswerService.Domain.Interfaces.Producer;
using Moq;

namespace AnswerService.Tests.UnitTests.Fixtures;

internal static class BaseEventProducerFixture
{
    public static IBaseEventProducer GetBaseEventProducerConfiguration()
    {
        var mockProducer = new Mock<IBaseEventProducer>();

        mockProducer.Setup(x =>
                x.ProduceAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<BaseEventType>(),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mockProducer.Object;
    }
}