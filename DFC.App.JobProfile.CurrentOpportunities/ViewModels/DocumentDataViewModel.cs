using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class DocumentDataViewModel
    {
        [Display(Name = "Standards")]
        public IEnumerable<string> Standards { get; set; }

        [Display(Name = "Frameworks")]
        public IEnumerable<string> Frameworks { get; set; }

        [Display(Name = "Course Keywords")]
        public string CourseKeywords { get; set; }

        [Display(Name = "Apprenticeships")]
        public IEnumerable<ApprenticeshipViewModel> Apprenticeships { get; set; }

        [Display(Name = "Courses")]
        public IEnumerable<CourseViewModel> Courses { get; set; }
    }
}
