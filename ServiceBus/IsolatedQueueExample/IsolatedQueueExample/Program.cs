using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using QueueExample.Services;


var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        // WARNING!  I have found no way to override connection string bindings for service bus in an ISOLATED Azure function.  
        //           This code seems to run AFTER the binding is checked and does NOT appear to override the connection string at all!

        // You might need this depending on your local dev environment
        // var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true });
        var credential = new DefaultAzureCredential();

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
                options.Connect(new Uri(azureAppConfigurationEndpoint), credential)
                    .ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(credential);
                    });
            });
        }
        // -------------------------------------End: App Configuration Example

        // -------------------------------------Start: Key Vault Example
        // Microsoft's current guidance for using Key Vault in an Azure Functions is to use the reference notation
        // https://docs.microsoft.com/en-us/azure/app-service/app-service-key-vault-references?toc=%2Fazure%2Fazure-functions%2Ftoc.json&tabs=azure-cli#reference-syntax
        // which looks like this (note how I did NOT include the version):
        // @Microsoft.KeyVault(SecretUri=https://myvault.vault.azure.net/secrets/mysecret/)
        // Note: This reference notation does NOT work locally.  It only works in the cloud configuration!!!

        // Required NuGet packages for direct key vault connection:
        // 1. Azure.Identity
        // 2. Azure.Extensions.AspNetCore.Configuration.Secrets 
        // Example: https://github.com/mizrael/AzureFunction-KeyVault/blob/main/Program.cs
        //builder.AddEnvironmentVariables();

        //var keyVaultEndpoint = Environment.GetEnvironmentVariable("AzureKeyVaultEndpoint");

        //if (string.IsNullOrWhiteSpace(keyVaultEndpoint) == false)
        //{
        //    builder.AddAzureKeyVault(new Uri(keyVaultEndpoint), credential);
        //}
        // -------------------------------------End: Key Vault Example
    })
    .ConfigureFunctionsWorkerDefaults(builder =>
    {

    })
    .ConfigureServices((builder, sc) =>
    {
        var configuration = builder.Configuration;
        
        // Note: ServiceBusConnectionString__fullyQualifiedNamespace is only used when you want to use your identity to talk to service bus.  
        //       It also requires the use of the  Microsoft.Azure.Functions.Worker.Extensions.ServiceBus NuGet package version >= 5.0.0; otherwise, 
        //       will not be picked up.
        var setting = new ServiceSettings
        {
            QueueFullyQualifiedNamespace = Environment.GetEnvironmentVariable("ServiceBusConnectionString__fullyQualifiedNamespace") ?? string.Empty,
            QueueConnectionString = configuration["ServiceBusConnectionString"], // configuration will be populated with both environment variables (default behavior) and app configuration (due to code below).
            QueueName = configuration["ServiceBusQueueName"]
        };

        sc.AddQueueExampleServices(setting);
    })
    .Build();

host.Run();
