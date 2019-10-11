using System;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels
{
    public class CurrentOpportunitiesPatchSegmentModel
    {
        public string CanonicalName { get; set; }

        public string SocLevelTwo { get; set; }

        public CurrentOpportunitiesSegmentDataModel Data { get; set; }
    }
}
