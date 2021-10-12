using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class DocumentDataViewModel
    {
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Display(Name = "Title Prefix")]
        public string JobTitleWithPrefix { get; set; }

        [Display(Name = "Content Title")]
        public string ContentTitle { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime LastReviewed { get; set; }

        [Display(Name = "Apprenticeships")]
        public DocumentApprenticeshipsViewModel Apprenticeships { get; set; }

        [Display(Name = "Courses")]
        public DocumentCoursesViewModel Courses { get; set; }
    }
}
