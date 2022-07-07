using FunctionDynamic.Models;
using Microsoft.Extensions.Logging;

namespace FunctionDynamic.Services
{
    public class RabbitService : IRabbitService
    {
        private readonly ILogger<RabbitService> _logger;

        public RabbitService(ILogger<RabbitService> logger)
        {
            _logger = logger;
        }

        public async Task<ExecutionResult> DoGoodWorkAsync(int jobId)
        {
            _logger.LogInformation($"Good Rabbit: {jobId}.");

            var result = new ExecutionResult();

            return await Task.FromResult(result);
        }

        public async Task<ExecutionResult> DoPartialWorkAsync(int jobId)
        {
            _logger.LogInformation($"Partial Rabbit: {jobId}.");

            var result = new ExecutionResult();
            result.TryMarkAsPartial("Partial failure log text");

            return await Task.FromResult(result);
        }

        public async Task<ExecutionResult> DoSoftFailureWorkAsync(int jobId)
        {
            _logger.LogInformation($"Soft failure Rabbit: {jobId}.");

            var result = new ExecutionResult();
            result.TryMarkAsFailure("Soft failure log text");

            return await Task.FromResult(result);
        }

        public async Task<ExecutionResult> DoHardFailureWorkAsync(int jobId)
        {
            throw new ArgumentException("This is a rabbit exception!");
        }

        public async Task<ExecutionResult> DoRollbackWorkAsync(int jobId)
        {
            _logger.LogInformation($"Rollback Rabbit: {jobId}.");

            var result = new ExecutionResult();
            result.TryMarkAsFailure("Rollback needed log text");

            return await Task.FromResult(result);
        }
    }
}
