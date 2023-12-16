using HttpTriggerExample.Model;
using HttpTriggerExample.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCoreIsolated;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExampleDependencies(this IServiceCollection sc, IConfiguration config)
    {
        // Options pattern: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0#the-options-pattern

        // If you need the values here in this extension, do this
        var exampleOptions = new ExampleOptions();
        config.Bind(exampleOptions);

        // If you need to register it with DI, use DI services to configure options: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0#use-di-services-to-configure-options
        sc.Configure<ExampleOptions>(config);
        

        sc.AddServices(config);

        return sc;
    }
}