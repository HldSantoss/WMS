using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infra.Message.Operations
{
    public class CreateStructure : ICreateStructure, IDisposable
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<CreateStructure> _logger;
        private readonly IModel _channel;

        public CreateStructure(IRabbitMQPersistentConnection persistentConnection, ILogger<CreateStructure> logger)
        {
            _persistentConnection = persistentConnection;
            _logger = logger;

            _channel = _channel = _persistentConnection.CreateModel();
        }

        public IModel CreateConsumerChannel(string exchangeName, string queueName, string routingKey, bool durable)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("trace - Creating RabbitMQ DLX");

            _channel.ExchangeDeclare(exchange: $"dlx.{exchangeName}",
                                    type: "fanout",
                                    durable: durable);

            var args = new Dictionary<string, object>();
            args.Add("x-queue-mode","lazy");
            args.Add("x-queue-type", "classic");
            _channel.QueueDeclare(queue: $"dlq.{queueName}",
                                durable: durable,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            _channel.QueueBind(queue: $"dlq.{queueName}",
                                exchange: $"dlx.{exchangeName}",
                                routingKey: routingKey);

            _logger.LogTrace("trace - Creating RabbitMQ consumer channel");

            _channel.ExchangeDeclare(exchange: exchangeName,
                                    type: "direct");

            var argsx = new Dictionary<string, object>();
            argsx.Add("x-dead-letter-exchange", $"dlx.{exchangeName}");
            argsx.Add("x-queue-mode","lazy");
            _channel.QueueDeclare(queue: queueName,
                                 durable: durable,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _channel.QueueBind(queue: $"{queueName}",
                                exchange: $"{exchangeName}",
                                routingKey: routingKey);

            _channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "warning - Recreating RabbitMQ consumer channel");
            };

            return _channel;
        }

        public void Dispose()
        {
            if (_channel != null)
            {
                _channel.Dispose();
            }
        }
    }
}