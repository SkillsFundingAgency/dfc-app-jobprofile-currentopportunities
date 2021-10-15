using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BodyApprenticeshipsViewModel
    {
        [Display(Name = "Apprenticeships")]
        public IEnumerable<BodyVacancyViewModel> Vacancies { get; set; }
    }
}
