using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace DotNetCoreIsolated;

public class FunctionSimplePost
{
    private readonly ILogger _logger;

    public FunctionSimplePost(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<FunctionSimpleGet>();
    }
     
    [Function("SimplePost")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic? data = JsonConvert.DeserializeObject(requestBody);
        string? greeting = data?.greeting;
        string? lastname = data?.lastname;


        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString($"Welcome to Azure Functions! {greeting ?? "Unspecified greeting"}, {lastname ?? "Unknown"}");

        return response;
    }
}