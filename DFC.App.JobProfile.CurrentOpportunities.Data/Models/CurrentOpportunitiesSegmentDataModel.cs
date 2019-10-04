using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class CurrentOpportunitiesSegmentDataModel
    {
        [Required]
        public DateTime LastReviewed { get; set; }

        public string JobTitle { get; set; }

        public Apprenticeships Apprenticeships { get; set; }

        public Courses Courses { get; set; }
    }
}
