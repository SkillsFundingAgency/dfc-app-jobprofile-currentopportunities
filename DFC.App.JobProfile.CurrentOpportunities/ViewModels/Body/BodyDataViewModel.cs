using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BodyDataViewModel
    {
        public string JobTitle { get; set; }

        public string JobTitleWithPrefix { get; set; }

        [Display(Name = "Apprenticeships")]
        public BodyApprenticeshipsViewModel Apprenticeships { get; set; }

        [Display(Name = "Courses")]
        public BodyCoursesViewModel Courses { get; set; }
    }
}
