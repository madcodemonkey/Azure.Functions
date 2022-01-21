using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using FunctionDynamic.Models;
using FunctionDynamic.Services;

namespace FunctionDynamic
{
    public class ActivityRabbit
    {
        private readonly IRabbitService _rabbitService;

        public ActivityRabbit(IRabbitService rabbitService)
        {
            _rabbitService = rabbitService;
        }

        [FunctionName(nameof(A_RabbitGood))]
        public async Task<ActivityData> A_RabbitGood([ActivityTrigger] ActivityData activityData, ILogger log)
        {
            var result = await _rabbitService.DoGoodWorkAsync(activityData.JobId);

            activityData.Merge(result);

            return activityData;
        }

        [FunctionName(nameof(A_RabbitPartial))]
        public async Task<ActivityData> A_RabbitPartial([ActivityTrigger] ActivityData activityData, ILogger log)
        {
            var result = await _rabbitService.DoPartialWorkAsync(activityData.JobId);

            activityData.Merge(result);

            return activityData;

        }

        [FunctionName(nameof(A_RabbitFailure))]
        public async Task<ActivityData> A_RabbitFailure([ActivityTrigger] ActivityData activityData, ILogger log)
        {
            var result = await _rabbitService.DoSoftFailureWorkAsync(activityData.JobId);

            activityData.Merge(result);

            return activityData;
        }

        [FunctionName(nameof(A_RabbitException))]
        public async Task<ActivityData> A_RabbitException([ActivityTrigger] ActivityData activityData, ILogger log)
        {
            var result = await _rabbitService.DoHardFailureWorkAsync(activityData.JobId);

            activityData.Merge(result);

            return activityData;
        }

        [FunctionName(nameof(A_RabbitRollback))]
        public async Task<ActivityData> A_RabbitRollback([ActivityTrigger] ActivityData activityData, ILogger log)
        {
            var result = await _rabbitService.DoRollbackWorkAsync(activityData.JobId);

            activityData.Merge(result);

            return activityData;
         
        }
    }
}