using System.Threading.Tasks;
using FunctionDynamic.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDynamic
{
    public class ActivityPapa
    {
        [FunctionName(nameof(A_Papa))]
        public async Task<ActivityData>  A_Papa([ActivityTrigger] ActivityData activityData, ILogger log)
        {
            log.LogInformation($"Papa: {activityData.JobId}.");
            return new ActivityData(activityData);
        }

       
    }
}