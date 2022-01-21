using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AppInsightsEnhancedWithSerilog.Services
{
    public class MyCoolService : IMyCoolService
    {
        private readonly ILogger<MyCoolService> _logger;

        public MyCoolService(ILogger<MyCoolService> logger)
        {
            _logger = logger;
        }

        public async Task<string> DoSomeWorkAsync(string jobId)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            int sleepInterval = rand.Next(1, 5) * 1000;

            _logger.LogInformation("We will sleep for {sleepIntervalInMillisecond} Millisecond", sleepInterval);
            _logger.LogWarning("This is a warning!");

            _logger.LogError(new ArgumentException("In service outer exception here",
                new FileNotFoundException("Inner exception", "SomeFileName.xml")),
                "This is a LogError with an exception");



            var position = new { Latitude = 25, Longitude = 134 };
            var elapsedMs = 34;
            var numbers = new int[] { 1, 2, 3, 4 };

            _logger.LogInformation("Processed {@Position} in {Elapsed:000} ms., str {str}, numbers: {numbers}", position, elapsedMs, "test", numbers);


            await Task.Delay(sleepInterval);

            return jobId;
        }
    }
}
