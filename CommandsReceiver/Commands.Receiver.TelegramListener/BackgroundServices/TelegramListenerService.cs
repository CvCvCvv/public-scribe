using CommandsReceiver.Application.Abstractions;
using CommandsReceiver.Application.Domain.Requests.GenerateStory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Commands.Receiver.TelegramListener.BackgroundServices
{
    public class TelegramListenerService : BackgroundService
    {
        private readonly TelegramBotClient _botClient;
        private readonly ReceiverOptions _receiverOptions;
        IRabbitMqService _rabbitMqService;
        public TelegramListenerService(IConfiguration configuration, IRabbitMqService rabbitMqService)
        {
            var e = configuration["Telegram:Token"];
            _botClient = new TelegramBotClient(configuration["Telegram:Token"]!);
            _receiverOptions = new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message } };
            _rabbitMqService = rabbitMqService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _botClient.OnMessage += OnMessageReceived;
            return Task.CompletedTask;
        }

        private async Task OnMessageReceived(Message message, UpdateType type)
        {
            switch (type)
            {
                case UpdateType.Message:
                    await PrepareMessage(message);
                    break;
                default:
                    break;
            }
        }

        private async Task PrepareMessage(Message message)
        {
            var e = message.MessageThreadId;
            if (message.Text != null && message.Text.StartsWith("/theme ") && message.IsTopicMessage && message.Chat.Title == "Scriber" && message.MessageThreadId == 2)
            {
                var theme = message.Text.Replace("/theme ", "");

                try
                {
                    _rabbitMqService.SendMessage(System.Text.Json.JsonSerializer.Serialize(new GenerateStoryRequest() { Theme = theme, Author = message.From != null ? message.From.FirstName : "Anonymus" }));
                    var position = _rabbitMqService.CountInQueue() + 1;

                    await _botClient.SendMessage(message.Chat.Id, $"Получил тему: \'{theme}\'. Отправил в очередь", replyParameters: new ReplyParameters() { MessageId = message.Id });
                }
                catch (Exception)
                {
                    await _botClient.SendMessage(message.Chat.Id, $"Произошла ошибка  `^`", replyParameters: new ReplyParameters() { MessageId = message.Id });
                }
            }
            else if (!message.IsTopicMessage)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Сюда писать не надо мне (");
            }
        }
    }
}
