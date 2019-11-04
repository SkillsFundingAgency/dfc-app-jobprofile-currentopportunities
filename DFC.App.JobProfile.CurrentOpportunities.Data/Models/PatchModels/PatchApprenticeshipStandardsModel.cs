using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels
{
    public class PatchApprenticeshipStandardsModel : BasePatchModel
    {
        public Guid SOCCodeClassificationId { get; set; }

        public string SocCode { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string JobProfileTitle { get; set; }
    }
}