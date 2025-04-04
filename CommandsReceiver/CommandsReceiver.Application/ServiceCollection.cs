using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CommandsReceiver.Application
{
    public static class ServiceCollection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            Assembly assembly = typeof(ServiceCollection).GetTypeInfo().Assembly;
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        }
    }
}
