using DotNetCoreIsolated;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(workerApplication =>
    {
        // https://docs.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#middleware
        // Register our custom middleware with the worker
        workerApplication.UseMiddleware<ExceptionMiddleware>();


        // Application Insights not supported in a Nuget package yet! It's mentioned as unsupported here at the bottom of this table:
        // https://docs.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#differences-with-net-class-library-functions
        // If you want to pull in an additional 2 or 3 projects, you could use the projects listed here:
        // https://github.com/Azure/azure-functions-dotnet-worker/tree/main/src
        // Note that DotNetWorker.ApplicationInsights references to DotNetWorker.Core and I believe DotNetWorker.Core references DotNetWorker
        // This NuGet package is in Preview: https://www.nuget.org/packages/Microsoft.Azure.Functions.Worker.Extensions.ApplicationInsights/

        // This may be the solution for Application Insights: https://docs.microsoft.com/en-us/azure/azure-monitor/app/worker-service#using-application-insights-sdk-for-worker-services
        // https://www.nuget.org/packages/Microsoft.ApplicationInsights.WorkerService


        //workerApplication.UseWhen<StampHttpHeaderMiddleware>((context) =>
        //{
        //    // We want to use this middleware only for http trigger invocations.
        //    return context.FunctionDefinition.InputBindings.Values
        //        .First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";
        //});
    })
    .ConfigureServices(s =>
    {
        s.AddDotNetCoreIsolatedDependencies();

    })
    .Build();

host.Run();
