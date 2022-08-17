using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace DotNetCoreNotIsolated;

public static class FunctionSimpleGet
{
    [FunctionName("SimpleGet")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)  
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        // Getting query parameters (Way 1)
        string? greeting1 = req.Query["greeting"];
        string? lastname1 = req.Query["lastname"];

        // Getting query parameters (Way 2)
        IDictionary<string, string> output = req.GetQueryParameterDictionary();
        string? greeting2 = output.ContainsKey("greeting") ? req.Query["greeting"] : (string?) null;
        string? lastname2 = output.ContainsKey("lastname") ? req.Query["lastname"] : (string?) null;

        string responseMessage = $"Welcome to Azure Functions! Parameter: {greeting1 ?? "Unspecified greeting"}, {lastname1 ?? "Unknown"}" +
            $" Parsed Query string:  {greeting2 ?? "Unspecified greeting"}, {lastname2 ?? "Unknown"}";

        return new OkObjectResult(responseMessage);
    }

    // Note that you cannot add greeting and last name to the method parameters like we did in the isolated function.
    // If you do, you'll get a binding error
    //public static IActionResult Run(
    //    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    //    ILogger log , string? greeting, string? lastname) // <-------------------------------Cannot add these last 2!

}