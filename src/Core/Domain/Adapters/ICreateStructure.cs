using RabbitMQ.Client;

namespace Domain.Adapters
{
    public interface ICreateStructure
    {
        IModel CreateConsumerChannel(string exchangeName, string queueName, string routingKey, bool durable);
    }
}