using CommandsReceiver.Application.Abstractions;
using CommandsReceiver.RabbitMQ.RabbitMQService;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsReceiver.RabbitMQ
{
    public static class ServiceCollection
    {
        public static void AddInfrastructureRabbitMq(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
        }
    }
}
