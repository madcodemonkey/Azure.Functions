//using Azure.Identity;
//using Microsoft.Azure.Functions.Extensions.DependencyInjection;
//using Microsoft.Extensions.Configuration;
//using QueueExample.Services;
//using System;

//[assembly: FunctionsStartup(typeof(QueueExample.Startup))]

//namespace QueueExample;

//public class Startup : FunctionsStartup
//{
//    public override void Configure(IFunctionsHostBuilder builder)
//    {
//        var configuration = builder.GetContext().Configuration;

//        var setting = new ServiceSettings
//        {
//            QueueConnectionString = configuration["ServiceBusConnectionString"], // configuration will be populated with both environment variables (default behavior) and app configuration (due to code below).
//            QueueName = configuration["ServiceBusQueueName"]
//        };

//        builder.Services.AddQueueExampleServices(setting);
//    }

//    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
//    {
//        // Microsoft's current guidance for Azure Functions that are in the cloud is to use the reference notation
//        // https://docs.microsoft.com/en-us/azure/app-service/app-service-key-vault-references?toc=%2Fazure%2Fazure-functions%2Ftoc.json&tabs=azure-cli#reference-syntax
//        // which looks like this (note that it EXCLUDES the version):
//        // @Microsoft.KeyVault(SecretUri=https://myvault.vault.azure.net/secrets/mysecret/)

//        var azureAppConfigurationEndpointUri = Environment.GetEnvironmentVariable("AzureAppConfigurationEndpoint");

//        if (!string.IsNullOrWhiteSpace(azureAppConfigurationEndpointUri))
//        {
//            // You might need this depending on your local dev env
//            // var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true });
//            var credentials = new DefaultAzureCredential();

//            // Requires the Microsoft.Extensions.Configuration.AzureAppConfiguration
//            builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
//            {
//                options.Connect(new Uri(azureAppConfigurationEndpointUri), credentials)
//                    .ConfigureKeyVault(kv =>
//                    {
//                        kv.SetCredential(credentials);
//                    });
//            });
//        }
//    }
//}
