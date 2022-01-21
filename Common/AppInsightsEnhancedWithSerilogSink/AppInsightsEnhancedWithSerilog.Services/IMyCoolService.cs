using System.Threading.Tasks;

namespace AppInsightsEnhancedWithSerilog.Services
{
    public interface IMyCoolService
    {
        Task<string> DoSomeWorkAsync(string jobId);
    }
}