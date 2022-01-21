using System;
using System.Collections.Generic;
using System.Text;

namespace AppInsightsEnhancedWithSerilog
{
    public class JobInfo
    {
        public JobInfo() { }
        public JobInfo(string correlationId, string jobId)
        {
            CorrelationId = correlationId;
            JobId = jobId;
        }

        public string CorrelationId { get; set; }
        public string JobId { get; set; }
    }
}
