using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using QueueExample;

// See Connect to App Configuration: https://docs.microsoft.com/en-us/azure/azure-app-configuration/quickstart-azure-functions-csharp?tabs=isolated-process#connect-to-an-app-configuration-store

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        var azureAppConfigurationEndpointUri = Environment.GetEnvironmentVariable("AzureAppConfigurationEndpoint");

        // You might need this depending on your local dev env
        // var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true });
        // Requires Azure.Identity Nuget package.
        var credentials = new DefaultAzureCredential();

        // Requires the Microsoft.Extensions.Configuration.AzureAppConfiguration
        builder.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(azureAppConfigurationEndpointUri), credentials)
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(credentials);
                });
        });
    })
    .ConfigureFunctionsWorkerDefaults(builder =>
    {

    })
    .ConfigureServices((builder, sc) =>
    {
        sc.AddQueueExamplesDependencies(builder.Configuration);
    })
    .Build();

host.Run();
