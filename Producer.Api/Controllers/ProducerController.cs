using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Producer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProducerController : ControllerBase
    {
        private readonly ILogger<ProducerController> _logger;

        public ProducerController(ILogger<ProducerController> logger)
        {
            _logger = logger;
        }

        [HttpPost("message")]
        public ActionResult CreateMessage([FromBody] dynamic message)
        {
            var factory = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@localhost:5672") };
        
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("demo_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            byte[] body = Encoding.UTF8.GetBytes(message.ToString());
            
            channel.BasicPublish("", "demo_queue", null, body);

            return Accepted();
        }
    }
}
