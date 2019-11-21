using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels
{
    public class CurrentOpportunitiesDeleteServiceBusModel
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public DateTime LastReviewed { get; set; }
    }
}
