using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _logger = logger;

            var factory = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@localhost:5672") };
        
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare("demo_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += (SetIndexBinder, e) => {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($" -- Message Received --  \n {message} \n");
            };

            _channel.BasicConsume("demo_queue", true, consumer);

            return Task.CompletedTask;
        }
    }
}
