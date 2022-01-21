using FunctionDynamic.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDynamic
{
    public class ActivityHeartbeat
    {
        [FunctionName(nameof(A_DoJobHeartbeat))]
        public void A_DoJobHeartbeat([ActivityTrigger] ActivityData activityData, ILogger log)
        {
            log.LogInformation($"Updating database with heartbeat: {activityData.JobId}.");
        }
    }
}