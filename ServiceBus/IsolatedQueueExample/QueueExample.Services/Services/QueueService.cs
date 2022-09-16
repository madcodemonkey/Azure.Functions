using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace QueueExample.Services;

public class QueueService : IQueueService
{
    private readonly ILogger<QueueService> _logger;
    private readonly ServiceSettings _settings;
    private ServiceBusClient? _client;
    
    /// <summary>Constructor</summary>
    public QueueService(ILogger<QueueService> logger, ServiceSettings settings)
    {
        _logger = logger;
        _settings = settings;
    }

    /// <summary>The service bus client.</summary>
    /// <remarks> I didn't put this in constructor because it does some work when constructed and I don't
    /// expect this class to be used 99% of the time when it's injected so I'm avoiding construction of the
    /// client till it is used by a call to the SendMessageAsync method.</remarks>
    private ServiceBusClient Client => _client ??= CreateServiceBusClient();

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


    /// <summary>Creates a service bus client based upon the settings.</summary>
    private ServiceBusClient CreateServiceBusClient()
    {
        if (string.IsNullOrWhiteSpace(_settings.QueueConnectionString) == false &&
            _settings.QueueConnectionString.Contains("SharedAccessKeyName"))
        {
            return new ServiceBusClient(_settings.QueueConnectionString);
        }

        if (string.IsNullOrWhiteSpace(_settings.QueueFullyQualifiedNamespace) == false)
        {
            return new ServiceBusClient(_settings.QueueFullyQualifiedNamespace, new DefaultAzureCredential());
        }

        throw new ArgumentException("The Service Bus connection string does not contain a SharedAccessKeyName, " +
            "so your intent must be to use a managed identity in the cloud and your identity to connect locally; " +
            "however, the Fully Qualified Namespace was not specified!");
    }

}