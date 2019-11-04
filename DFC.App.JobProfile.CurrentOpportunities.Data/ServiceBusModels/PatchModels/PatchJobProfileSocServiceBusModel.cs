using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels.PatchModels
{
    public class PatchJobProfileSocServiceBusModel : BaseJobProfileMessage
    {
        public Guid Id { get; set; }

        public string SocCode { get; set; }

        public string Description { get; set; }

        public string ONetOccupationalCode { get; set; }

        public string UrlName { get; set; }

        public string Title { get; set; }

        public string JobProfileTitle { get; set; }

        public IEnumerable<ApprenticeshipFramework> ApprenticeshipFramework { get; set; }

        public IEnumerable<ApprenticeshipStandard> ApprenticeshipStandards { get; set; }
    }
}