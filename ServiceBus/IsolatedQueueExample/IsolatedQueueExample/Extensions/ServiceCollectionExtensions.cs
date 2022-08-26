using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QueueExample.Services;

namespace QueueExample;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQueueExamplesDependencies(this IServiceCollection sc, IConfiguration configuration)
    {

        var setting = new ServiceSettings()
        {
            QueueConnectionString = configuration["ServiceBusConnectionString"], // configuration will be populated with both environment variables (default behavior) and app configuration (due to code below).
            QueueName = configuration["ServiceBusQueueName"]
        };

        sc.AddSingleton(setting);
        sc.AddScoped<IQueueService, QueueService>();
        return sc;
    }
}