using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class DocumentDataViewModel
    {
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Display(Name = "Title Prefix")]
        public string TitlePrefix { get; set; }

        [Display(Name = "Content Title")]
        public string ContentTitle { get; set; }

        [Display(Name = "Last Reviewed")]
        public DateTime LastReviewed { get; set; }

        [Display(Name = "Apprenticeships")]
        public DocumentApprenticeshipsViewModel Apprenticeships { get; set; }

        [Display(Name = "Courses")]
        public DocumentCoursesViewModel Courses { get; set; }
    }
}
