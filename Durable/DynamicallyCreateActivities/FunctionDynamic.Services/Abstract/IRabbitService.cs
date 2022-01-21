using System.Threading.Tasks;
using FunctionDynamic.Models;

namespace FunctionDynamic.Services
{
    public interface IRabbitService
    {
        Task<ExecutionResult> DoGoodWorkAsync(int jobId);
        Task<ExecutionResult> DoPartialWorkAsync(int jobId);
        Task<ExecutionResult> DoSoftFailureWorkAsync(int jobId);
        Task<ExecutionResult> DoHardFailureWorkAsync(int jobId);
        Task<ExecutionResult> DoRollbackWorkAsync(int jobId);
    }
}