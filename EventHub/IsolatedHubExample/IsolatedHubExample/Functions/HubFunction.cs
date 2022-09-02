using HubExample.Model;
using HubExample.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IsolatedHubExample;

public class HubFunction
{
    private readonly IHubService _hubService;
    private readonly ILogger _logger;

    public HubFunction(ILoggerFactory loggerFactory, IHubService hubService)
    {
        _hubService = hubService;
        _logger = loggerFactory.CreateLogger<HubFunction>();
    }

    [Function("HubFunction")]
    public async Task Run([EventHubTrigger("%EventHubName%", Connection = "EventHubConnectionString")] string[] input, 
        FunctionContext context)
    {
        foreach (string oneInput in input)
        {
            _logger.LogInformation($"C# Event Hub trigger function processed a message: {oneInput}");

            Deal? someDeal = JsonConvert.DeserializeObject<Deal>(oneInput);
            if (someDeal != null && someDeal.IsGoodDeal == false)
            {
                someDeal.IsGoodDeal = true;
                await _hubService.SendMessageAsync(someDeal); // Put it back on the hub (it will be processed immediately) How to delay?

                _logger.LogWarning($"Bad deal re-queued: {oneInput}");
            }
            else
            {
                _logger.LogInformation($"Good deal: {oneInput}");
            }            
        }
    }
}