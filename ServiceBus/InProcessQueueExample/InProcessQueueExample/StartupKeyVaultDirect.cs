using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using QueueExample.Services;
using System;

[assembly: FunctionsStartup(typeof(QueueExample.Startup))]

namespace QueueExample;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = builder.GetContext().Configuration;

        var setting = new ServiceSettings
        {
            QueueConnectionString = configuration["ServiceBusConnectionString"], // configuration will be populated with both environment variables and key vault due to code below.
            QueueName = configuration["ServiceBusQueueName"]
        };

        builder.Services.AddQueueExampleServices(setting);
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        var builtConfig = builder.ConfigurationBuilder.Build();

        var configuration = builder.ConfigurationBuilder
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables(); // Writes environmental variables into IConfiguration and will overwrite key vault entries if done before the next line.

        // Microsoft's current guidance for Azure Functions that are in the cloud is to use the reference notation
        // https://docs.microsoft.com/en-us/azure/app-service/app-service-key-vault-references?toc=%2Fazure%2Fazure-functions%2Ftoc.json&tabs=azure-cli#reference-syntax
        // which looks like this (note that it EXCLUDES the version):
        // @Microsoft.KeyVault(SecretUri=https://myvault.vault.azure.net/secrets/mysecret/)

        // Do we want to use key vault to retrieve things locally?
        var keyVaultEndpoint = builtConfig["AzureKeyVaultEndpoint"];

        if (string.IsNullOrWhiteSpace(keyVaultEndpoint) == false)
        {
            // You might need this depending on your local dev env
            // var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true });
            var credential = new DefaultAzureCredential();

            configuration
                // Writes vault entries into IConfiguration only and NOT environmental variables
                // Requires the Azure.Extensions.AspNetCore.Configuration.Secrets NuGet package
                .AddAzureKeyVault(new Uri(keyVaultEndpoint), credential);
        }

        configuration.Build();
    }
}
