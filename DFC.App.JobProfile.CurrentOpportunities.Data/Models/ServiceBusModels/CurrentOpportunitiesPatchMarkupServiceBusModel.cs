using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models.ServiceBusModels
{
    public class CurrentOpportunitiesPatchMarkupServiceBusModel
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        [Required]
        public CurrentOpportunitiesSegmentDataModel Data { get; set; }
    }
}
