using DotNetCoreIsolated.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCoreIsolated;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDotNetCoreIsolatedDependencies(this IServiceCollection sc)
    {

        sc.AddServices();

        return sc;
    }
}