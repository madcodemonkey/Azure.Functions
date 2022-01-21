using FunctionDynamic.Services.Extensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(FunctionDynamic.Startup))]

namespace FunctionDynamic
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDynamicServices(new FunctionDynamicSettings());
        }
    }
}
