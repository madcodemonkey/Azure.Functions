using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using QueueExample.Services;


var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        // You might need this depending on your local dev env
        // var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true });
        var credentials = new DefaultAzureCredential();

        // -------------------------------------Start: App Configuration Example
        // Required NuGet packages for Azure App Configuration:
        // 1. Azure.Identity
        // 2. Microsoft.Extensions.Configuration.AzureAppConfiguration
        // Docs: https://docs.microsoft.com/en-us/azure/azure-app-configuration/quickstart-azure-functions-csharp?tabs=isolated-process#connect-to-an-app-configuration-store

        var azureAppConfigurationEndpoint = Environment.GetEnvironmentVariable("AzureAppConfigurationEndpoint");

        if (string.IsNullOrWhiteSpace(azureAppConfigurationEndpoint) == false)
        {
            builder.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(azureAppConfigurationEndpoint), credentials)
                    .ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(credentials);
                    });
            });
        }
        // -------------------------------------End: App Configuration Example

        // -------------------------------------Start: Key Vault Example
        // Required NuGet packages for direct key vault connection:
        // 1. Azure.Identity
        // 2. Azure.Extensions.AspNetCore.Configuration.Secrets 
        // Example: https://github.com/mizrael/AzureFunction-KeyVault/blob/main/Program.cs
        //builder.AddEnvironmentVariables();

        //var keyVaultEndpoint = Environment.GetEnvironmentVariable("AzureKeyVaultEndpoint");

        //if (string.IsNullOrWhiteSpace(keyVaultEndpoint) == false)
        //{
        //    builder.AddAzureKeyVault(new Uri(keyVaultEndpoint), credentials);
        //}
        // -------------------------------------End: Key Vault Example
    })
    .ConfigureFunctionsWorkerDefaults(builder =>
    {

    })
    .ConfigureServices((builder, sc) =>
    {
        var configuration = builder.Configuration;

        var setting = new ServiceSettings
        {
            QueueConnectionString = configuration["ServiceBusConnectionString"], // configuration will be populated with both environment variables (default behavior) and app configuration (due to code below).
            QueueName = configuration["ServiceBusQueueName"]
        };

        sc.AddQueueExampleServices(setting);
    })
    .Build();

host.Run();
