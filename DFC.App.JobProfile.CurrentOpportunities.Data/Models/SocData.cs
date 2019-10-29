using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class SocData
    {
        public Guid Id { get; set; }

        public IEnumerable<ApprenticeshipFramework> ApprenticeshipFramework { get; set; }

        public IEnumerable<ApprenticeshipStandard> ApprenticeshipStandards { get; set; }
    }
}