using System.Threading.Tasks;
using FunctionDynamic.Models;
using Microsoft.Extensions.Logging;

namespace FunctionDynamic.Services
{
    public class PapaService : IPapaService
    {
        private readonly ILogger<PapaService> _logger;

        public PapaService(ILogger<PapaService> logger)
        {
            _logger = logger;
        }

        public async Task<ExecutionResult> CallYourDadAsync()
        {
            _logger.LogInformation("Papa service called");

            var result = new ExecutionResult();
            result.TryMarkAsSuccess("Said papa");
            
            return await Task.FromResult(result);
        }
         
    }
}