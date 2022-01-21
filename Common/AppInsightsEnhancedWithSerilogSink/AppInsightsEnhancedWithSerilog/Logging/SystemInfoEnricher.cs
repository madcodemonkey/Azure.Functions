using System;
using System.IO;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;

namespace AppInsightsEnhancedWithSerilog
{
    /// <summary>Enriches logging information with data about the machine, namespace, etc.</summary>
    public class SystemInfoEnricher : ILogEventEnricher
    {
        /// <summary>Enriches logging information with data about the machine, namespace, etc.</summary>
        /// <param name="logEvent">The logging event you want to enrich before storing it.</param>
        /// <param name="propertyFactory">Used to create properties on the <paramref name="logEvent"/> instance.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            try
            {

                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Computer", Environment.MachineName));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("AppName", LogInfo.AppName));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId", LogInfo.CorrelationId));

                if (string.IsNullOrWhiteSpace(LogInfo.JobId) == false)
                {
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("JobId", LogInfo.JobId));
                }

                if (LogInfo.FunctionAssembly != null)
                {
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("AssemblyFullName", LogInfo.FunctionAssembly.FullName));
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("AssemblyVersion", LogInfo.FunctionAssembly.GetName().Version.ToString()));
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("AssemblyBuildDate", AssemblyBuildDate(LogInfo.FunctionAssembly).ToString()));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>Retrieves the build date time on an assembly.</summary>
        /// <param name="objAssembly">The assembly to examine and pull a last write time date from.</param>
        /// <returns>A date time if possible; otherwise, the max date value.</returns>
        public static DateTime AssemblyBuildDate(Assembly objAssembly)
        {
            try
            {
                return File.GetLastWriteTime(objAssembly.Location);
            }
            catch (Exception)
            {
                return DateTime.MaxValue;
            }
        }
    }
}