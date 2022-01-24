using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ServiceBusSendMessage
{
    class Program
    {
        static string myServiceBusConnectionString = "===Your Connection string here===";

        // name of your Service Bus topic
        static string topicName = "davetopic";

        // the client that owns the connection and can be used to create senders and receivers
        static ServiceBusClient client;

        // the sender used to publish messages to the topic
        static ServiceBusSender sender;

        // number of messages to be sent to the topic
        private const int numOfMessages = 1;

        static async Task Main()
        {
            // Using connection string (simplest) 
            client = new ServiceBusClient(myServiceBusConnectionString);

            // Managed Identity (way 1)  
            // string fullyQualifiedNamespace = "yournamespacehere.servicebus.windows.net";
            // var defaultAzureCredential = new DefaultAzureCredential();
            // client = new ServiceBusClient(fullyQualifiedNamespace, defaultAzureCredential);
            // Note 1:  I did create an environment variable (see project > Debug tab (environment variables) called AZURE_TENANT_ID and then added my tenant id as a value.

            // Managed Identity (way 2)
            // You must have "Azure Service Bus Data Owner" or "Azure Service Bus Data Sender" role for this to work.  
            // string myTenantId = "56670c5c-985e-4c5b-9061-d7f270c7937f";
            //TokenCredential tokenCredential = new VisualStudioCredential(new VisualStudioCredentialOptions { TenantId = myTenantId });
            //client = new ServiceBusClient(fullyQualifiedNamespace, tokenCredential);


            sender = client.CreateSender(topicName);

            // create a batch 
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            for (int i = 1; i <= numOfMessages; i++)
            {
                var data = new DataToSend($"Mickey Mouse{i}", "https://www.google.com");

                string serializeObject = JsonConvert.SerializeObject(data);
                ServiceBusMessage message = new ServiceBusMessage(serializeObject);
                message.ContentType = "application/json;charset=utf-8";
                message.ApplicationProperties.Add("WebUrl", data.Url);

                // try adding a message to the batch
                if (!messageBatch.TryAddMessage(message))
                {
                    // if it is too large for the batch
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus topic
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} messages has been published to the topic.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            Console.WriteLine("Press any key to end the application");
            Console.ReadKey();
        }
    }
}
