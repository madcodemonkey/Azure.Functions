using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QueueExample.Model;
using QueueExample.Services;

namespace QueueExample
{
    public class QueueFunction
    {
        private readonly IQueueService _queueService;
        private readonly ILogger _logger;

        public QueueFunction(ILoggerFactory loggerFactory, IQueueService queueService)
        {
            _queueService = queueService;
            _logger = loggerFactory.CreateLogger<QueueFunction>();
        }

        // Binding example: https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-service-bus-trigger?tabs=isolated-process%2Cfunctionsv2&pivots=programming-language-csharp#example
        [Function("YatesQueueTrigger")]
        public async Task Run([ServiceBusTrigger("%ServiceBusQueueName%", Connection = "ServiceBusConnectionString")] string myQueueItem,
            Int32 deliveryCount, DateTime enqueuedTimeUtc, string messageId, FunctionContext context)
        {
            Deal? someDeal = JsonConvert.DeserializeObject<Deal>(myQueueItem);
            if (someDeal != null && someDeal.IsGoodDeal == false)
            {
                someDeal.IsGoodDeal = true;
                await _queueService.SendMessageAsync(someDeal, delayInSeconds: 120); // Put it back on the queue with a 2 minute delay

                _logger.LogWarning($"Bad deal re-queued: {myQueueItem}");
            }
            else
            {
                _logger.LogInformation($"Good deal: {myQueueItem}");
            }
        }
    }
}
