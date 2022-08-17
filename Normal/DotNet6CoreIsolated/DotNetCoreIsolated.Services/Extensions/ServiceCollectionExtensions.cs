using Microsoft.Extensions.DependencyInjection;

namespace DotNetCoreIsolated.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection sc, ServiceSettings settings)
    {
        sc.AddSingleton(settings);

        sc.AddScoped<IMyExceptionCreatorService, MyExceptionCreatorService>();
        sc.AddScoped<IMyMathService, MyMathService>();

        return sc;
    }
}