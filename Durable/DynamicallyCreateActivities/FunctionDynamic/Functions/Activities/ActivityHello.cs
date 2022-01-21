using System.Threading.Tasks;
using FunctionDynamic.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDynamic
{
    public class ActivityHello
    {

        [FunctionName(nameof(A_Hello))]
        public async Task<ActivityData>  A_Hello([ActivityTrigger] ActivityData activityData, ILogger log)
        {
            log.LogInformation($"Hello: {activityData.JobId}.");
            return new ActivityData(activityData);
        }
    }
}