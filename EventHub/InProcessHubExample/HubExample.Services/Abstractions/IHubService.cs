namespace HubExample.Services;

public interface IHubService
{
    /// <summary>Sends a message to a queue.</summary>
    /// <param name="dataToSerialize">The class instance to serialize</param>
    Task SendMessageAsync<T>(T dataToSerialize) where T : class;

    /// <summary>Sends a message to a queue.</summary>
    /// <param name="messageData">The message to send</param>
    Task SendMessageAsync(string messageData);
}