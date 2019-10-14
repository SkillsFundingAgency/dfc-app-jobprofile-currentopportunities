using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class HealthItemViewModel
    {
        public string Service { get; set; }

        public string SubService { get; set; }

        public string Message { get; set; }

        public HealthServiceState HealthServiceState { get; set; }

        public string CheckParametersUsed { get; set; }
    }
}
