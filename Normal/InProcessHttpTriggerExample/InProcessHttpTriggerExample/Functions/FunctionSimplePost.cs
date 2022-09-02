using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace DotNetCoreNotIsolated;

public static class FunctionSimplePost
{
    [FunctionName("SimplePost")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        string? greeting = data?.greeting;
        string? lastname = data?.lastname;

        string responseMessage = $"Welcome to Azure Functions! {greeting ?? "Unspecified greeting"}, {lastname ?? "Unknown"}";

        return new OkObjectResult(responseMessage);
    }
}