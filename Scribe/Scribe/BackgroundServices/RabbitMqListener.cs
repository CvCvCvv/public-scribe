using Application.Abstractions.Domains.Models.GenerateStory;
using Application.Abstractions.Domains;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scribe.Service;
using System.Text;
using System.Text.Json;

namespace Scribe.BackgroundServices
{
    internal class RabbitMqListener
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;
        private const string _streamerQueue = "StreamerCannel";
        private const string _storyQueue = "myQueue";
        public RabbitMqListener()
        {
            var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _storyQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            using var _channelSteramer = _connection.CreateModel();
            _channelSteramer.QueueDeclare(queue: _streamerQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine("Начал работу");

            _consumer = new EventingBasicConsumer(_channel);

            _consumer.Received += (sender, args) =>
            {

                var content = JsonSerializer.Deserialize<GenerateStoryModel>(Encoding.UTF8.GetString(args.Body.ToArray()))!;

                Console.WriteLine($"Получил тему: {content.Theme}, автор: {content.Author}");

                try
                {
                    ShapeStory.Shape(content).Wait();

                    //_channel.BasicAck(args.DeliveryTag, false);

                    var streamerStoryInfo = new StreamerStoryInfoModel() { Story = content, Id = Guid.Parse(RenPyCodeHelper.RootStoryPath) };
                    _channelSteramer.BasicPublish("", _streamerQueue, null, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(streamerStoryInfo)));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error!! Theme:{content.Theme}, Author:{content.Author}");
                    Console.WriteLine(e);

                    //_channel.BasicReject(deliveryTag: args.DeliveryTag, requeue: true);
                }

            };

            _channel.BasicConsume("myQueue", true, _consumer);
        }
    }
}
