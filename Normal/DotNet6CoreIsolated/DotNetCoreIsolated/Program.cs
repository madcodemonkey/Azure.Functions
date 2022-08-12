using DotNetCoreIsolated;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        // https://docs.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#middleware
        // Register our custom middleware with the worker
        builder.UseMiddleware<ExceptionMiddleware>();

        // Application Insights open issue
        // https://github.com/Azure/azure-functions-dotnet-worker/issues/760

        // Application Insights not supported in a Nuget package yet! It's mentioned as unsupported here at the bottom of this table:
        // https://docs.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#differences-with-net-class-library-functions
        // If you want to pull in an additional 2 or 3 projects, you could use the projects listed here:
        // https://github.com/Azure/azure-functions-dotnet-worker/tree/main/src
        // Note that DotNetWorker.ApplicationInsights references to DotNetWorker.Core and I believe DotNetWorker.Core references DotNetWorker
        // This NuGet package is in Preview: https://www.nuget.org/packages/Microsoft.Azure.Functions.Worker.Extensions.ApplicationInsights/

        // This may be the solution for Application Insights: https://docs.microsoft.com/en-us/azure/azure-monitor/app/worker-service#using-application-insights-sdk-for-worker-services
        // https://www.nuget.org/packages/Microsoft.ApplicationInsights.WorkerService
        
        //builder.UseWhen<StampHttpHeaderMiddleware>((context) =>
        //{
        //    // We want to use this middleware only for http trigger invocations.
        //    return context.FunctionDefinition.InputBindings.Values
        //        .First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";
        //});
    })
    .ConfigureServices(s =>
    {
        s.AddLogging();
        
        // Requires Microsoft.ApplicationInsights.WorkerService NuGet package.
        // Despite what you read here, a connection string uses APPLICATIONINSIGHTS_CONNECTION_STRING within the values object.
        // You can see this if you create a new Azure Function App in the portal and specify you want Application Insights.
        // https://docs.microsoft.com/en-us/azure/azure-monitor/app/worker-service#net-core-lts-worker-service-application
        //  s.AddApplicationInsightsTelemetryWorkerService("92c3da69-756b-46f7-9ad9-2c3ac4ca0908");
        s.AddApplicationInsightsTelemetryWorkerService();


        s.AddDotNetCoreIsolatedDependencies();
    })
    .Build();

host.Run();
