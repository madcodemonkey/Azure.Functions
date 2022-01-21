using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AppInsightsEnhancedWithSerilog.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AppInsightsEnhancedWithSerilog
{
    public class TheFunction
    {
        private readonly IMyCoolService _coolService;

        /// <summary>Constructor</summary>
        public TheFunction(IMyCoolService coolService)
        {
            _coolService = coolService;
        }

        [FunctionName(nameof(RunOrchestratorAsync))]
        public async Task RunOrchestratorAsync([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            LogInfo.AppName = "AppInsights Enhanced with serilog";
            LogInfo.CorrelationId = "11602AFB-E2F1-46CA-B659-22F5AAC28F86";
          
            var taskList = new List<Task>();

            if (context.IsReplaying == false) log.LogCritical(new ArgumentException("In Orchestrator outer exception here", new FileNotFoundException("Inner exception", "SomeFileName.xml")), "This is a LogCritical with an exception");


            if (context.IsReplaying == false) log.LogWarning("In orchestrator before calls");

            // Fan out
            taskList.Add(context.CallActivityAsync(nameof(DoWorkAsync), new JobInfo(LogInfo.CorrelationId, "d6eb0868-7aa5-4e10-befa-2173510279c4")));
            taskList.Add(context.CallActivityAsync(nameof(DoWorkAsync), new JobInfo(LogInfo.CorrelationId, "00e9602d-7a2b-4b6b-aeeb-7155711ecd0c")));
            taskList.Add(context.CallActivityAsync(nameof(DoWorkAsync), new JobInfo(LogInfo.CorrelationId, "d7b64992-5811-407e-aeb8-36d02dab2e6c")));
            
            // Fan im
           await Task.WhenAll(taskList);

           if (context.IsReplaying == false) log.LogWarning("In orchestrator after calls");


        }

        [FunctionName(nameof(DoWorkAsync))]
        public async Task DoWorkAsync([ActivityTrigger] JobInfo job, ILogger log)
        {
            LogInfo.AppName = "AppInsights Enhanced with serilog";
            LogInfo.CorrelationId = job.CorrelationId;
            LogInfo.JobId = job.JobId;

            log.LogCritical(new ArgumentException("In activity outer exception here", new FileNotFoundException("Inner exception", "SomeFileName.xml")), "This is a LogCritical with an exception");

            log.LogWarning("In activity before call");
            
            await _coolService.DoSomeWorkAsync(job.JobId);
            
            log.LogWarning("In activity after call");

        }

        [FunctionName(nameof(RunStuff))]
        public async Task<HttpResponseMessage> RunStuff(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync(nameof(RunOrchestratorAsync), null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}