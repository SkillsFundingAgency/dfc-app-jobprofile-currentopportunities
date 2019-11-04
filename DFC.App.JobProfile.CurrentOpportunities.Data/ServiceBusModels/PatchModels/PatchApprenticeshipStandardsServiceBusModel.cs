using System;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels.PatchModels
{
    public class PatchApprenticeshipStandardsServiceBusModel : BaseJobProfileMessage
    {
        public Guid Id { get; set; }

        public Guid SOCCodeClassificationId { get; set; }

        public string SocCode { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string JobProfileTitle { get; set; }
    }
}