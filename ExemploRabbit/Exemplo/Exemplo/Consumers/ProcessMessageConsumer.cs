using Exemplo.Models;
using Exemplo.Options;
using Exemplo.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exemplo.Consumers
{
    public class ProcessMessageConsumer : BackgroundService
    {
        private readonly RabbitConfig _configuration;
        private readonly QueueConfig _queue;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        public ProcessMessageConsumer(IOptions<RabbitConfig> rabbitConf, IOptions<QueueConfig> queueConf, IServiceProvider serviceProvider)
        {
            _configuration = rabbitConf.Value;
            _queue = queueConf.Value;
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                HostName = _configuration.HostName,
                Port = _configuration.Port,
                Password = _configuration.Password,
                UserName = _configuration.UserName
            };
            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                        queue: _queue.PrefixForRetriable,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, eventArgs) =>
            {
                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);
                var message = JsonConvert.DeserializeObject<MessageModel>(contentString);

                NotifyUser(message);

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(_queue.PrefixForRetriable, false, consumer);

            return Task.CompletedTask;
        }

        public void NotifyUser(MessageModel message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                notificationService.NotifyUser(message);
            }
        }
    }
}
