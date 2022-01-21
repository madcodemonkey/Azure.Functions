namespace FunctionDynamic.Models
{
    public class ActivityData : ExecutionResult
    {
        public ActivityData() { }

        public ActivityData(ActivityData data)
        {
            JobId = data.JobId;
        }

        public int JobId { get; set; }
        
    }
}