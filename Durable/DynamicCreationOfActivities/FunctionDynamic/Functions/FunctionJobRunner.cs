using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FunctionDynamic
{
    public class FunctionJobRunner
    {
        [FunctionName(nameof(JobRunner))]
        public async Task<IActionResult> JobRunner(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            int jobId;
            if (int.TryParse(req.Query["jobId"], out jobId) == false)
            {
                jobId = 4;
            }

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync(nameof(OrchestratorMain.MainOrchestrator), null, jobId);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return new OkObjectResult($"Running job id {jobId}");
        }
    }
}