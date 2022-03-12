using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Exemplo.Options;
using System.Text;
using Exemplo.Models;

namespace Exemplo.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController: ControllerBase
    {
        private readonly ConnectionFactory _factory;
        private readonly QueueConfig _queue;
        private readonly RabbitConfig _config;
        public MessagesController(IOptions<RabbitConfig> options, IOptions<QueueConfig> queueConf)
        {
            _config = options.Value;
            _queue = queueConf.Value;

            _factory = new ConnectionFactory
            {
                HostName = _config.HostName
            };
        }

        [HttpPost]
        public IActionResult PostMessage([FromBody] MessageModel message)
        {
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var stringfiedMessage = JsonConvert.SerializeObject(message);
                    var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: _queue.PrefixForRetriable,
                        basicProperties: null,
                        body: bytesMessage);
                }
            }

            return Accepted();
        }
    }
}
