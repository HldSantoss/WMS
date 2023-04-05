using System.Net.Security;
using Infra.Message.Operations;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infra.Message
{
    public static class RabbitmqModuleDependency
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = Convert.ToBoolean(configuration["RabbitMQ:Connection:Ssl"]) 
                    ? ConnFactoryRabbitmqSsl(configuration) 
                    : ConnFactoryRabbitmqNonSsl(configuration);

                factory.UserName = configuration["RabbitMQ:Connection:UserName"];
                factory.Password = configuration["RabbitMQ:Connection:Password"];
                factory.Port = Convert.ToInt32(configuration["RabbitMQ:Connection:Port"]);

                return new DefaultRabbitMQPersistentConnection(factory, logger, int.Parse(configuration["RabbitMQ:Connection:RetryCount"]));
            });

            services.AddSingleton<IPublisher, Publisher>();
            services.AddScoped<ICreateStructure, CreateStructure>();

            return services;
        }

        private static ConnectionFactory ConnFactoryRabbitmqSsl(IConfiguration configuration)
        {
            return new ConnectionFactory()
                {
                    HostName = configuration["RabbitMQ:Connection:HostName"],
                    DispatchConsumersAsync = true,
                    Ssl = new SslOption()
                    {
                        ServerName = configuration["RabbitMQ:Connection:HostName"],
                        Enabled = true,
                        AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors
                    }
                };
        }

        private static ConnectionFactory ConnFactoryRabbitmqNonSsl(IConfiguration configuration)
        {
            return new ConnectionFactory()
                {
                    HostName = configuration["RabbitMQ:Connection:HostName"],
                    DispatchConsumersAsync = true
                };
        }
    }
}