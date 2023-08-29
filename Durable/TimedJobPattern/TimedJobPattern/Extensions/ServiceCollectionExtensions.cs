using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimedJobPattern.Services;

namespace DotNetCoreIsolated;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add services needed for DI.
    /// </summary>
    public static IServiceCollection AddTimedJobPatternDependencies(this IServiceCollection sc, IConfiguration config)
    {
        sc.AddServices(config);

        return sc;
    }
}