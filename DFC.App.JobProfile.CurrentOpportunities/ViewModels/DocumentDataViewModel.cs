using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class DocumentDataViewModel
    {
        public DateTime Updated { get; set; }

        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Display(Name = "Last Reviewed")]
        public DateTime LastReviewed { get; set; }

        public IEnumerable<string> Standards { get; set; }

        public IEnumerable<string> Frameworks { get; set; }

        [Display(Name = "Course Keywords")]
        public string CourseKeywords { get; set; }

        [Display(Name = "Apprenticeships")]
        public IEnumerable<ApprenticeshipViewModel> Apprenticeships { get; set; }

        [Display(Name = "Courses")]
        public IEnumerable<CourseViewModel> Courses { get; set; }
    }
}
