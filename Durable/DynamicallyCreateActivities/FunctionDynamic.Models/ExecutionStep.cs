namespace FunctionDynamic.Models
{
    public class ExecutionStep 
    {
        public ExecutionStep()
        {
        }

        public ExecutionStep(string primaryActivityFunctionName, string rollbackActivityFunctionName, 
            int? retryFirstIntervalInMinutes = null, int? retryMaxNumberOfAttempts = null)
        {
            PrimaryActivityFunctionName = primaryActivityFunctionName;
            RollbackActivityFunctionName = rollbackActivityFunctionName;
            RetryMaxNumberOfAttempts = retryMaxNumberOfAttempts;
            RetryFirstIntervalInMinutes = retryFirstIntervalInMinutes;
        }

        public string PrimaryActivityFunctionName { get; set; }
        public string RollbackActivityFunctionName { get; set; }

        /// <summary>An orchestrator has the ability to retry thing if exceptions are thrown.  This specified the retry first interval in minutes.</summary>
        public int? RetryFirstIntervalInMinutes { get; set; }

        /// <summary>An orchestrator has the ability to retry thing if exceptions are thrown.  This specified the retry maximum number of attempts.</summary>
        public int? RetryMaxNumberOfAttempts { get; set; }
    }
}