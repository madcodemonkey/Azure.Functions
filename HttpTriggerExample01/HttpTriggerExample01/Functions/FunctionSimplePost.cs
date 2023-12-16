using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using HttpTriggerExample.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace HttpTriggerExample.Functions;

public class FunctionSimplePost
{
    private readonly ILogger _logger;

    public FunctionSimplePost(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<FunctionSimplePost>();
    }

    [Function("FunctionSimplePost")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        // If you want to deal with dynamics
        dynamic? data = JsonSerializer.Deserialize<JsonNode>(requestBody);
        string? greeting = (string?)data?["greeting"];
        string? lastname = (string?)data?["lastname"];
        // Otherwise, use a concrete class
        var greetInfo = JsonSerializer.Deserialize<GreetInfo>(requestBody,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });


        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        await response.WriteStringAsync($"Welcome to Azure Functions! " +
                                        $"Dynamics: {greeting ?? "Unspecified greeting"}, {lastname ?? "Unknown last name"} ||||" +
                                        $"Concrete: {greetInfo?.Greeting ?? "Unspecified greeting"}, {greetInfo?.Lastname ?? "Unknown last name"}");


        return response;
    }
}