using Microsoft.Extensions.DependencyInjection;

namespace HubExample.Services;

public static class ServiceCollectionExtension
{
    public static void AddHubExampleServices(this IServiceCollection sc, ServiceSettings settings)
    {
        sc.AddSingleton(settings);
        sc.AddScoped<IHubService, HubService>();
    }
}