using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class ServiceHealthStatus
    {
        public string Service { get; set; }

        public string SubService { get; set; }

        public string Message { get; set; }

        public HealthServiceState HealthServiceState { get; set; }

        public string CheckParametersUsed { get; set; }
    }
}
