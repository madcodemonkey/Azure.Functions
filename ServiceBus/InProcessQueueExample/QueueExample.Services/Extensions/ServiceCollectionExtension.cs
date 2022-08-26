using Microsoft.Extensions.DependencyInjection;

namespace QueueExample.Services
{
    public static class ServiceCollectionExtension
    {
        public static void AddQueueExampleServices(this IServiceCollection sc, ServiceSettings settings)
        {
            sc.AddSingleton(settings);
            sc.AddScoped<IQueueService, QueueService>();

        }
    }
}
