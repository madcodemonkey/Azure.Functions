# Isolated Queue Example
This Azure Function has two functions
1. One hooks up to an existing service bus.  Currently it will receive the Deal.cs class (under QueueExample.Model library) from the service bus queue specified in the appsettings.json file.
2. One has an HttpTrigger for pulling secrets from either Azure Key Vault or Azure App Configuration.  This is intended only to show that you are hooked up.

## local.settings.json changes you need to make
1. Update ServiceBusConnectionString with your Service Bus connection string 
2. Update AZURE_TENANT_ID to your tenant id.  You'll find that info your Azure Active Driectory "Overview".
3. If you want configuration to come from Azure Key vault, uncomment and update AzureKeyVaultEndpoint (see Key Vault below for more info) 
4. If you want configuratoin to come from Azure App Confifguration, uncomment and update AzureAppConfigurationEndpoint (see App Configuration below for more info) 

## Warnings
The host evaluates special bindings like ServiceBusTrigger, EventHubTrigger, etc. before the program.cs file is even run, so with connections and other information used by the binding there are limitations:
- When testing/debugging locally, this information MUST come from your local.settings.json file!
- In the cloud, you CAN use the key vault REFERENCE syntax: @Microsoft.KeyVault(SecretUri=https://myvault.vault.azure.net/secrets/mysecret/)  
- You CANNOT use Azure App Configuration to configure these values.

## Program.cs changes you need to make 
You should do one of the following 
1. Comment out both the Key Vault or App Configuration because you only intend to use the stuff in local.settings.json.
2. Comment out Key Vault code because you intented to access everything via App Configuration
3. Comment out App Configuration code because you intented to access everything via Key Vault

## Azure Key Vault change you need to make
1. You'll need to give yourself permission to access the Azure Key Vault for this to work.  This is done under "Access Policies"
2. You'll need to add some secrets if you want to pull them out via the HttpTrigger.

## Azure App Configuration change you need to make
1. You'll need to create a managed identity for your App Configuration.
2. You'll have to give that managed identity permission to access your key vault
3. You'll need to give yourself "App Configuration Data Reader" permission to access the App Configuration for this to work.  This is done under "Access Policies"
4. You'll have to add secrets from your key vault via the "Configuration Explorer" Create button (you can create a ""key-value"" or "key vault reference")



