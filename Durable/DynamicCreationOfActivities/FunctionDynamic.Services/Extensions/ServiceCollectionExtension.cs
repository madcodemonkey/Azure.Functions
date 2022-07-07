using Microsoft.Extensions.DependencyInjection;

namespace FunctionDynamic.Services.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddDynamicServices(this IServiceCollection sc, FunctionDynamicSettings settings)
        {
            sc.AddSingleton(settings);

            sc.AddScoped<IHelloService, HelloService>();
            sc.AddScoped<IPapaService, PapaService>();
            sc.AddScoped<IRabbitService, RabbitService>();
        }
    }
}
