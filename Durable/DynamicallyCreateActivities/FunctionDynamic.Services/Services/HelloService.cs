using System.Threading.Tasks;
using FunctionDynamic.Models;
using Microsoft.Extensions.Logging;

namespace FunctionDynamic.Services
{
    public class HelloService : IHelloService
    {
        private readonly ILogger<HelloService> _logger;

        public HelloService(ILogger<HelloService> logger)
        {
            _logger = logger;
        }

        public async Task<ExecutionResult> SayHelloAsync(string helloText)
        {
            _logger.LogInformation(helloText);

            var result = new ExecutionResult();
            result.TryMarkAsSuccess("Said hello");
            
            return await Task.FromResult(result);
        }
    }
}