using System;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels
{
    public class ApprenticeshipFramework
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }
    }
}
