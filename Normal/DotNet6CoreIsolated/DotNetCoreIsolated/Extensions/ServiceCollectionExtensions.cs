using DotNetCoreIsolated.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCoreIsolated;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDotNetCoreIsolatedDependencies(this IServiceCollection sc)
    {
        var serviceSettings = new ServiceSettings { RunInformation = Environment.GetEnvironmentVariable("RunInformation") };

        sc.AddServices(serviceSettings);

        return sc;
    }
}