using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QueueExample.Services;
using System;
using System.Threading.Tasks;
using QueueExample.Model;

namespace QueueExample
{
    public class QueueFunction
    {
        private readonly IQueueService _queueService;

        public QueueFunction(IQueueService queueService)
        {
            _queueService = queueService;
        }

        // Binding example: https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-service-bus-trigger?tabs=in-process%2Cfunctionsv2&pivots=programming-language-csharp#example
        [FunctionName("YatesQueueTrigger")]
        public async Task Run([ServiceBusTrigger("%ServiceBusQueueName%", Connection = "ServiceBusConnectionString")] string myQueueItem,
            Int32 deliveryCount, DateTime enqueuedTimeUtc, string messageId, ILogger log)
        {
            Deal someDeal = JsonConvert.DeserializeObject<Deal>(myQueueItem);
            if (someDeal != null && someDeal.IsGoodDeal == false)
            {
                someDeal.IsGoodDeal = true;
                await _queueService.SendMessageAsync(someDeal, delayInSeconds: 30); // Put it back on the queue with a 30 second delay

                log.LogWarning($"Bad deal re-queued: {myQueueItem}");
            }
            else
            {
                log.LogInformation($"Good deal: {myQueueItem}");
            }
        }
    }
}
