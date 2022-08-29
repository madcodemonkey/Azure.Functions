using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace QueueExample.Services;

public class QueueService : IQueueService
{
    private readonly ServiceSettings _settings;
    private ServiceBusClient? _client;
    
    /// <summary>Constructor</summary>
    public QueueService(ServiceSettings settings)
    {
        _settings = settings;
    }

    /// <summary>The service bus client.</summary>
    /// <remarks> I didn't put this in constructor because it does some work when constructed and I don't
    /// expect this class to be used 99% of the time when it's injected so I'm avoiding construction of the
    /// client till it is used by a call to the SendMessageAsync method.</remarks>
    private ServiceBusClient Client => _client ??= new ServiceBusClient(_settings.QueueConnectionString);

    /// <summary>Sends a message to a queue.</summary>
    /// <param name="dataToSerialize">The class instance to serialize</param>
    /// <param name="delayInSeconds">The number of seconds to delay the message</param>
    public async Task SendMessageAsync<T>(T dataToSerialize, int delayInSeconds) where T : class
    {
        await SendMessageAsync(JsonConvert.SerializeObject(dataToSerialize), delayInSeconds);
    }

    /// <summary>Sends a message to a queue.</summary>
    /// <param name="messageData">The message to send</param>
    /// <param name="delayInSeconds">The number of seconds to delay the message</param>
    public async Task SendMessageAsync(string messageData, int delayInSeconds)
    {
        await using var sender = Client.CreateSender(_settings.QueueName);

        var message = new ServiceBusMessage(messageData)
        {
            ScheduledEnqueueTime = new DateTimeOffset(DateTime.UtcNow.AddSeconds(delayInSeconds))
        };

        await sender.SendMessageAsync(message);
    }

}