# In-process Hub Example
This Azure Function has two functions
1. One hooks up to an existing event hub.  Currently it will receive the Deal.cs class (under HubExample.Model library) from the event hub specified in the local.settings.json file.
2. One has an HttpTrigger for pulling secrets from either Azure Key Vault or Azure App Configuration.  This is intended only to show that you are hooked up.

## local.settings.json changes you need to make
1. Update AZURE_TENANT_ID to your tenant id.  You'll find that info your Azure Active Driectory "Overview".
2. Update EventHubName with the name of your event hub
3. If you don't want to use key vault or Azure App Configuration for connection string info, update EventHubConnectionString with your Event Hub connection string 
4. If you want configuration to come from Azure Key vault, uncomment and update AzureKeyVaultEndpoint (see Key Vault below for more info) 
5. If you want configuratoin to come from Azure App Confifguration, uncomment and update AzureAppConfigurationEndpoint (see App Configuration below for more info) 

## Startup.cs changes you need to make 
You should do one of the following 
1. Comment out both the **Azure Key Vault** or **Azure App Configuration** because you only intend to use the stuff in local.settings.json.
2. Comment out **Azure Key Vault** and uncomment **Azure App Configuration** code because you intented to access everything via **Azure App Configuration**
3. Comment out **Azure App Configuration** and uncomment **Azure Key Vault** code because you intented to access everything via **Azure Key Vault**

## Azure Key Vault changes you need to make
1. You'll need to give yourself permission to access the Azure Key Vault for this to work.  This is done under "Access Policies"
2. You'll need to add some secrets if you want to pull them out via the HttpTrigger.
3. If you want the connection string for the EventHubTrigger binding to come from key vault, add a secret called EventHubConnectionString and update the value to be a connection string to your event hub.

## Azure App Configuration changes you need to make
1. You'll need to create a managed identity for your App Configuration.
2. You'll have to give that managed identity permission to access your key vault
3. You'll need to give yourself "App Configuration Data Reader" permission to access the App Configuration for this to work.  This is done under "Access Policies"
4. You'll have to add secrets from your key vault via the "Configuration Explorer" Create button (you can create a ""key-value"" or "key vault reference")
5. If you want the connection string for the binding to come from App Configuration, add it and update the connect string.


