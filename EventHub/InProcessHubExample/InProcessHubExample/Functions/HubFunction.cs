using Azure.Messaging.EventHubs;
using HubExample.Model;
using HubExample.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace InProcessHubExample
{
    public class HubFunction
    {
        private readonly IHubService _hubService;

        public HubFunction(IHubService hubService)
        {
            _hubService = hubService;
        }

        [FunctionName("HubFunction")]
        public async Task Run([EventHubTrigger("%EventHubName%", Connection = "EventHubConnectionString")] EventData[] events, ILogger log)
        {
            foreach (EventData eventData in events)
            {
                // Replace these two lines with your processing logic.
                log.LogInformation($"C# Event Hub trigger function processed a message: {eventData.EventBody}");
                string oneInput = Encoding.UTF8.GetString(eventData.EventBody);

                Deal? someDeal = JsonConvert.DeserializeObject<Deal>(oneInput);
                if (someDeal != null && someDeal.IsGoodDeal == false)
                {
                    someDeal.IsGoodDeal = true;
                    await _hubService.SendMessageAsync(someDeal); // Put it back on the hub (it will be processed immediately) How to delay?

                    log.LogWarning($"Bad deal re-queued: {oneInput}");
                }
                else
                {
                    log.LogInformation($"Good deal: {oneInput}");
                }
            }

        }
    }
}
