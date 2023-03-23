using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BodyOpportunityViewModel
    {
        public string Title { get; set; }

        public string CourseId { get; set; }

        public string RunId { get; set; }

        [Display(Name = "Provider")]
        public string Provider { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public bool FlexibleStartDate { get; set; }

        [Display(Name = "Location")]
        public LocationViewModel Location { get; set; }

        public string Url { get; set; }
    }
}
