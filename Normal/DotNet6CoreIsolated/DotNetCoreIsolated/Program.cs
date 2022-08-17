using DotNetCoreIsolated;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        // https://docs.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#middleware
        // Register our custom middleware with the worker
        builder.UseMiddleware<ExceptionMiddleware>();
    })
    .ConfigureServices(s =>
    {
        s.AddLogging(logging =>
        {
            // REQUIRES: Microsoft.ApplicationInsights.WorkerService NuGet package or the AddApplicationInsights extension will NOT be available!
            // Trace logs lacks a stack trace logs only the text passed to LogError(ex, thisText), but you can find the stack trace in the Exception logs
            // Works with either a APPLICATIONINSIGHTS_CONNECTION_STRING or APPINSIGHTS_INSTRUMENTATIONKEY
            // Note: There are also changes to the host.json file control logging.
            logging.AddApplicationInsights();
        });
       
        s.AddDotNetCoreIsolatedDependencies();
    })
    .Build();

host.Run();
