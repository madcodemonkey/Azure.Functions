using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using QueueExample;


var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        // You might need this depending on your local dev env
        // var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true });
        var credentials = new DefaultAzureCredential();

        // Required NuGet packages for Azure App Configuration:
        // 1. Azure.Identity
        // 2. Microsoft.Extensions.Configuration.AzureAppConfiguration

        var azureAppConfigurationEndpointUri = Environment.GetEnvironmentVariable("AzureAppConfigurationEndpoint");

        // Docs: https://docs.microsoft.com/en-us/azure/azure-app-configuration/quickstart-azure-functions-csharp?tabs=isolated-process#connect-to-an-app-configuration-store
        builder.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(azureAppConfigurationEndpointUri), credentials)
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(credentials);
                });
        });

        // Required NuGet packages for direct key vault connection:
        // 1. Azure.Identity
        // 2. Azure.Extensions.AspNetCore.Configuration.Secrets 

        // Example: https://github.com/mizrael/AzureFunction-KeyVault/blob/main/Program.cs
        //builder.AddEnvironmentVariables();

         //           var tmpConfig = builder.Build();
         //           var vaultUri = tmpConfig["AzureKeyVaultEndpoint"];
         //           if(string.IsNullOrWhiteSpace(vaultUri)){
         //               throw new ArgumentException("please provide a valid VaultUri");
         //           }

         //           builder.AddAzureKeyVault(new Uri(vaultUri), credentials);

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
