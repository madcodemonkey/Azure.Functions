using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttpTriggerExample.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection sc, IConfiguration config)
    {
        // Options pattern: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0#the-options-pattern

        // If you need the values here in this extension, do this
        var someOptions = new ServiceOptions();
        config.GetSection(ServiceOptions.SectionName).Bind(someOptions);

        // If you need to register it with DI, use DI services to configure options: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0#use-di-services-to-configure-options
        sc.Configure<ServiceOptions>(config.GetSection(ServiceOptions.SectionName));
         

        sc.AddScoped<IMyExceptionCreatorService, MyExceptionCreatorService>();
        sc.AddScoped<IMyMathService, MyMathService>();

        return sc;
    }
}