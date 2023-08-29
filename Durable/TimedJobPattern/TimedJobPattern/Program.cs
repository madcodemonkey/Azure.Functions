using DotNetCoreIsolated;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
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
