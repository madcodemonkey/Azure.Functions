using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Newtonsoft.Json;

namespace HubExample.Services
{
    public class HubService : IHubService
    {
        private readonly ServiceSettings _settings;
        private EventHubProducerClient? _client;
    
        /// <summary>Constructor</summary>
        public HubService(ServiceSettings settings)
        {
            _settings = settings;
        }

        /// <summary>The service bus client.</summary>
        /// <remarks> I didn't put this in constructor because it does some work when constructed and I don't
        /// expect this class to be used 99% of the time when it's injected so I'm avoiding construction of the
        /// client till it is used by a call to the SendMessageAsync method.</remarks>
        private EventHubProducerClient Client => _client ??= new EventHubProducerClient(_settings.EventHubConnectionString, _settings.EventHubName);

        /// <summary>Sends a message to a queue.</summary>
        /// <param name="dataToSerialize">The class instance to serialize</param>
        public async Task SendMessageAsync<T>(T dataToSerialize) where T : class
        {
            await SendMessageAsync(JsonConvert.SerializeObject(dataToSerialize));
        }
        
        
        /// <summary>Sends a message to a queue.</summary>
        /// <param name="messageData">The message to send</param>
        public async Task SendMessageAsync(string messageData)
        {
            using EventDataBatch eventBatch = await Client.CreateBatchAsync();

            byte[] data =  Encoding.UTF8.GetBytes(messageData);
          
            if (! eventBatch.TryAdd(new EventData(data)))
            {
                // if it is too large for the batch
                throw new Exception($"Event is too large for the batch and cannot be sent.");
            }
              
            await Client.SendAsync(eventBatch);
        }

        ///// <summary>Sends a message to a queue.</summary>
        ///// <param name="messageData">The message to send</param>
        //public async Task SendMessageAsync(string messageData)
        //{


        //    byte[] data = Encoding.UTF8.GetBytes(messageData);

        //    var message = new EventData(data);

        //    // SendEventOptions options = new SendEventOptions();

        //    await Client.SendAsync(new List<EventData> { message });
        //}
    }
}