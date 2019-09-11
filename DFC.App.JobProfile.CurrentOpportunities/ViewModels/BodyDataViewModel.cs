using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class BodyDataViewModel
    {
        [Display(Name = "Apprenticeships")]
        public IEnumerable<ApprenticeshipViewModel> Apprenticeships { get; set; }

        [Display(Name = "Courses")]
        public IEnumerable<CourseViewModel> Courses { get; set; }

        public Uri CourseSearchUrl { get; set; }
    }
}
