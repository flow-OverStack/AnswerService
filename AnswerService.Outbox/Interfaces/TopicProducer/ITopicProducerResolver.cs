namespace AnswerService.Outbox.Interfaces.TopicProducer;

public interface ITopicProducerResolver
{
    ITopicProducer GetProducerForType(Type messageType);
}