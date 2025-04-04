using CommandsReceiver.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CommandsReceiver.RabbitMQ.RabbitMQService
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly string _hostname;
        private readonly string _queue;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqService(IConfiguration configuration)
        {
            _hostname = configuration["RabbitMQ:HostName"]!;
            _queue = configuration["RabbitMQ:Queue"]!;

            var factory = new ConnectionFactory() { HostName = _hostname };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _queue, durable: false, exclusive: false, autoDelete: false);
        }

        public int CountInQueue()
        {
            return (int)_channel.MessageCount(_queue);
        }

        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish("", _queue, null, body);
        }
    }
}
