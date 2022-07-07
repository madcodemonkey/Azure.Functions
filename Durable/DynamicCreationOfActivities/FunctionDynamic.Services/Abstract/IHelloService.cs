using FunctionDynamic.Models;

namespace FunctionDynamic.Services
{
    public interface IHelloService
    {
        Task<ExecutionResult> SayHelloAsync(string helloText);
    }
}