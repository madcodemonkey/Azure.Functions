using System.Threading.Tasks;
using FunctionDynamic.Models;

namespace FunctionDynamic.Services
{
    public interface IPapaService
    {
        Task<ExecutionResult> CallYourDadAsync();
    }
}