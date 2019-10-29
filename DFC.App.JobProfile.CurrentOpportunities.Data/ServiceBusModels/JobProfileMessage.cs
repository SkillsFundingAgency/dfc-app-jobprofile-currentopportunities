using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels
{
    public class JobProfileMessage : BaseJobProfileMessage
    {
        [Required]
        public string CanonicalName { get; set; }

        public DateTime LastModified { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        public string CourseKeywords { get; set; }

        public SocCodeData SocCodeData { get; set; }
    }
}