using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class DocumentApprenticeshipsViewModel
    {
        public IEnumerable<string> Standards { get; set; }

        public IEnumerable<string> Frameworks { get; set; }

        [Display(Name = "Apprenticeships")]
        public IEnumerable<DocumentVacancyViewModel> Vacancies { get; set; }
    }
}
