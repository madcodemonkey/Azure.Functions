using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using QueueExample.Services;
using System;

[assembly: FunctionsStartup(typeof(QueueExample.Startup))]

namespace QueueExample;

public class Startup : FunctionsStartup
{
    // Note: This is called AFTER ConfigureAppConfiguration
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = builder.GetContext().Configuration;

        var setting = new ServiceSettings
        {
            QueueConnectionString = configuration["ServiceBusConnectionString"], // configuration will be populated with both environment variables (default behavior) and app configuration (due to code below).
            QueueName = configuration["ServiceBusQueueName"]
        };

        builder.Services.AddQueueExampleServices(setting);
    }

    // Note: This is called BEFORE Configure
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        // You might need this depending on your local dev environment
        // var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true });
        var credential = new DefaultAzureCredential();

        // -------------------------------------Start: App Configuration Example
        // Required NuGet packages for Azure App Configuration:
        // 1. Azure.Identity
        // 2. Microsoft.Extensions.Configuration.AzureAppConfiguration
        var azureAppConfigurationEndpoint = Environment.GetEnvironmentVariable("AzureAppConfigurationEndpoint");

        if (string.IsNullOrWhiteSpace(azureAppConfigurationEndpoint) == false)
        {
            builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
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

        // Required NuGet packages for Azure App Configuration:
        // 1. Azure.Identity
        // 2. Azure.Extensions.AspNetCore.Configuration.Secrets
        //var keyVaultEndpoint = Environment.GetEnvironmentVariable("AzureKeyVaultEndpoint");

        //if (string.IsNullOrWhiteSpace(keyVaultEndpoint) == false)
        //{
        //    builder.ConfigurationBuilder
        //        // Writes vault entries into IConfiguration only and NOT environmental variables
        //        // Requires the Azure.Extensions.AspNetCore.Configuration.Secrets NuGet package
        //        .AddAzureKeyVault(new Uri(keyVaultEndpoint), credential);
        //}
        // -------------------------------------End: Key Vault Example
    }

}
