using AppInsightsEnhancedWithSerilog.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;

[assembly: FunctionsStartup(typeof(AppInsightsEnhancedWithSerilog.Startup))]
namespace AppInsightsEnhancedWithSerilog
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = TelemetryConfiguration.Active;  //  This doesn't work:  TelemetryConfiguration.CreateDefault()
            if (config != null)
            {
                // Notes
                // Log and LoggerConfiguration         --> requires Serilog
                // CompactJsonFormatter                --> requires Serilog.Formatting.Compact
                // WriteTo.Console                     --> requires Serilog.Sinks.Console
                // WriteTo.ApplicationInsights         --> requires Serilog.Sinks.ApplicationInsights  (see also https://github.com/serilog/serilog-sinks-applicationinsights)
                var logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.ControlledBy(GetLoggingLevelSwitch())
                    .Enrich.With<SystemInfoEnricher>()
                    .WriteTo.Console(new CompactJsonFormatter())
                    .WriteTo.ApplicationInsights(config, TelemetryConverter.Traces)
                    //.WriteTo.ApplicationInsights(config, TelemetryConverter.Events)
                    .CreateLogger();

                Log.Logger = logger;

                builder.Services.AddLogging(lb => lb.AddSerilog(logger, dispose: true));
            }


            builder.Services.AddTransient<IMyCoolService, MyCoolService>();
        }

        /// <summary>Obtains the log level from configuration or defaults to Verbose</summary>
        private LoggingLevelSwitch GetLoggingLevelSwitch()
        {
            string level = Environment.GetEnvironmentVariable("MinimumLogLevel", EnvironmentVariableTarget.Process) ?? "Verbose";

            try
            {
                var levelEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), level);

                return new LoggingLevelSwitch(levelEventLevel);
            }
            catch
            {
                Console.WriteLine($"Unable to parse log level: {level}");
            }

            return new LoggingLevelSwitch(LogEventLevel.Verbose);
        }
    }
}

