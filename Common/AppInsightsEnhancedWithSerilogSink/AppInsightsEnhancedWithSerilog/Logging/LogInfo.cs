using System.Reflection;
using System.Threading;

namespace AppInsightsEnhancedWithSerilog
{
    /// <summary>An in-memory location to store information related to logging.</summary>
   public static class LogInfo
   {
       private static readonly AsyncLocal<string> _correlationId = new AsyncLocal<string>();
       private static readonly AsyncLocal<string> _jobId = new AsyncLocal<string>();
       private static readonly AsyncLocal<Assembly> _functionAssembly = new AsyncLocal<Assembly>();
       private static readonly AsyncLocal<string> _appName = new AsyncLocal<string>();

       /// <summary>Gets or sets a correlation id, which is used by a logging web job to enrich your logging messages.</summary>
       public static string CorrelationId
       {
           get => _correlationId.Value;
           set => _correlationId.Value = value;
       }

       /// <summary>Gets or sets a business id, which is used by a logging web job to enrich your logging messages.</summary>
       public static string JobId
       {
           get => _jobId.Value;
           set => _jobId.Value = value;
       }

       /// <summary>Gets or sets a function assembly instance, which is used by a logging web job to enrich your logging messages.</summary>
       public static Assembly FunctionAssembly
       {
           get => _functionAssembly.Value;
           set => _functionAssembly.Value = value;
       }

       /// <summary>Gets or sets a app ame, which is used by a logging web job to enrich your logging messages.</summary>
       public static string AppName
       {
           get => _appName.Value;
           set => _appName.Value = value;
       }
   }
}
