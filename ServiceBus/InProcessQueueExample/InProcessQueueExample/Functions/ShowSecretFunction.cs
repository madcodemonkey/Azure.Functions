using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace InProcessQueueExample.Functions
{
    public class ShowSecretFunction
    {
        private readonly IConfiguration _configuration;

        public ShowSecretFunction(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("ShowSecretFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string keyName = req.Query["keyName"];

            string? data = _configuration[keyName];

            string responseMessage = data == null ? $"Key named {keyName} was not Found" : $"{keyName}={data}";

            return new OkObjectResult(responseMessage);
        }
    }
}
