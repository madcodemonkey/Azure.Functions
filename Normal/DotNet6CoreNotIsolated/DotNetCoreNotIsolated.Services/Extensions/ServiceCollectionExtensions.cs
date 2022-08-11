using Microsoft.Extensions.DependencyInjection;

namespace DotNetCoreNotIsolated.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection sc)
    {
        sc.AddScoped<IMyExceptionCreatorService, MyExceptionCreatorService>();
        sc.AddScoped<IMyMathService, MyMathService>();

        return sc;
    }
}