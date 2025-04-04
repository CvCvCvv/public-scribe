using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Streamer.Application.Abstractions.Models.GenerateStory;
using System.Text;
using System.Text.Json;

namespace Streamer.BackgroundServices.RabbitMq
{
    public class RabbitMqListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly string _myQueue;
        private readonly Timer _timer;
        private readonly int _checkTime;
        private string locked = "_locked";

        public RabbitMqListener(IConfiguration configuration)
        {
            RenPyRunner.RenPyRunner.PathScenarios = configuration["RenPy:Scenarios"]!;
            RenPyRunner.RenPyRunner.PathRenPyGame = configuration["RenPy:GameFolder"]!;
            RenPyRunner.RenPyRunner.NameExeFile = configuration["RenPy:ExeFile"]!;
            RenPyRunner.RenPyRunner.PathExeFile = configuration["RenPy:PathExeFile"]!;
            RenPyRunner.RenPyRunner.Timeout = Convert.ToInt32(configuration["RenPy:ExitTimeout"]!);
            _checkTime = (int)TimeSpan.FromMinutes(Convert.ToInt32(configuration["CheckPeriod"]!)).TotalMilliseconds;

            _myQueue = configuration["RabbitMQ:Queue"]!;
            var factory = new ConnectionFactory { HostName = configuration["RabbitMQ:HostName"] };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _myQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            TimerCallback timerCallback = new TimerCallback(CheckCountMessage!);

            _timer = new Timer(timerCallback, null, 0, _checkTime);
        }

        private void CheckCountMessage(object obj)
        {
            if (_channel.MessageCount(_myQueue) <= 0)
            {
                Console.WriteLine("Нет историй в очереди, выбираем рандомную историю...");

                var story = GetRandStory();

                if (story != null)
                    _channel.BasicPublish("", _myQueue, null, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(story)));
            }
        }

        private StreamerStoryInfoModel? GetRandStory()
        {
            var rnd = new Random();
            var dirs = Directory.GetDirectories(RenPyRunner.RenPyRunner.PathScenarios).Where(a=> !a.EndsWith(locked)).ToArray();

            if (dirs.Length > 0)
            {
                var story = dirs[rnd.Next(0, dirs.Length)];

                var metadata = new StreamerStoryInfoModel();
                metadata.Story = JsonSerializer.Deserialize<GenerateStoryModel>(File.ReadAllText(Path.Combine(story, "meta.data")))!;
                var id = Path.GetFileName(story)!;
                metadata.Id = Guid.Parse(id);

                return metadata;
            }
            else
                return null;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    var content = JsonSerializer.Deserialize<StreamerStoryInfoModel>(Encoding.UTF8.GetString(ea.Body.ToArray()))!;

                    await RenPyRunner.RenPyRunner.Run(content);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка воспроизведения: {e.Message}");
                    _channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: true);
                }

            };

            _channel.BasicConsume(_myQueue, false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();

            _channel.Close();
            _connection.Close();
            _timer.Dispose();
        }
    }
}
