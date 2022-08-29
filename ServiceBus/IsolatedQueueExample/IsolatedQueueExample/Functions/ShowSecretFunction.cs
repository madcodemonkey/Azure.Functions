using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IsolatedQueueExample.Functions
{
    public class ShowSecretFunction
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public ShowSecretFunction(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<ShowSecretFunction>();
        }

        [Function("ShowSecretFunction")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, string keyName)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string? data = _configuration[keyName];

            string responseMessage = data == null ? $"Key named {keyName} was not Found" : $"{keyName}={data}";

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString(responseMessage);

            return response;
        }
    }
}
