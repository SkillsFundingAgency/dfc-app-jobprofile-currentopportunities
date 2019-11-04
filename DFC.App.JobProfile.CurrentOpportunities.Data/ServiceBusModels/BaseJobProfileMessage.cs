using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels
{
    public abstract class BaseJobProfileMessage
    {
        [Required]
        public Guid JobProfileId { get; set; }
    }
}