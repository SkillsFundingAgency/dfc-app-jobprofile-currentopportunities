using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels
{
    public class PatchSocDataModel : BasePatchModel
    {
        public string SocCode { get; set; }

        public IEnumerable<ApprenticeshipFramework> ApprenticeshipFramework { get; set; }

        public IEnumerable<ApprenticeshipStandard> ApprenticeshipStandards { get; set; }
    }
}