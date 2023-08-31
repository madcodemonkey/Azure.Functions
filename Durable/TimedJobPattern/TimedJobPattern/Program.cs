using DotNetCoreIsolated;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TimedJobPattern;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults((hostContext, builder) =>
    {
        // https://docs.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#middleware
        // Register our custom middleware with the worker
        builder.UseMiddleware<ExceptionMiddleware>();
    })
    .ConfigureAppConfiguration((hostContext, builder) =>
    {
        // Reference: https://stackoverflow.com/q/74269765/97803
        if (hostContext.HostingEnvironment.IsDevelopment())
        {
            builder.AddJsonFile("local.settings.json");
            builder.AddUserSecrets<Program>();
        }
    })
    .ConfigureServices((hostContext, s) =>
    {
        var configuration = hostContext.Configuration;

        s.AddTimedJobPatternDependencies(configuration);
    })
    .Build();

host.Run();
