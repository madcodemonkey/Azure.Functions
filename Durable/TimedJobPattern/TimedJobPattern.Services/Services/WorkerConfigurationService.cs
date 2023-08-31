using TimedJobPattern.Modals;

namespace TimedJobPattern.Services;

public class WorkerConfigurationService : IWorkerConfigurationService
{
    /// <summary>
    /// Computes the next time the worker should run.
    /// </summary>
    /// <param name="workerConfig">The worker's configuration</param>
    public DateTime ComputeNextWorkerRun(WorkerConfiguration workerConfig)
    {
        if (workerConfig.RunTimeIntervalInMinutes < 1)
        {
            throw new ArgumentException("The run time interval cannot be less than 1 minute!", nameof(workerConfig));
        }

        DateTime result = workerConfig.NextRunTime;

        if (workerConfig.AddIntervalToNextRunTime)
        {
            // If we've been down for a long time...go ahead and catch up the next run time till we surpass the current UTC time.
            while (result < DateTime.UtcNow)
            {
                result = result.AddMinutes(workerConfig.RunTimeIntervalInMinutes);
            }

            // Determine if we should jump up another interval or leave it alone.

            var timeDifference = result - DateTime.UtcNow;
            var percentageComplete = 1.00 - timeDifference.TotalMinutes / workerConfig.RunTimeIntervalInMinutes;
            if (percentageComplete > 0.5)
            {
                // We are over 50% through the interval, so let's go ahead an push out to the next interval.
                // Example 1: NextRunTime 14:00 and current time 13:35 and interval of 60 minutes.
                //            25 minutes before the next interval so we are 1 - 25/60 = 0.58 so we have already
                //            waited for 58% of the interval, so we will push to the next interval 15:00.
                // Example 2: NextRunTime 14:00 and current time 13:25 and interval of 60 minutes.
                //            35 minutes before the next interval so we are 1 - 35/60 = 0.42 so we have already
                //            waited for 42% of the interval, so we not adjust the interval so it will run again at 14:00.
                result = result.AddMinutes(workerConfig.RunTimeIntervalInMinutes);
            }

        }
        else
        {
            // You want the code to finish and then wait the ENTIRE interval before moving on to the next interval.
            result = DateTime.UtcNow.AddMinutes(workerConfig.RunTimeIntervalInMinutes);
        }

        return result;
    }
}