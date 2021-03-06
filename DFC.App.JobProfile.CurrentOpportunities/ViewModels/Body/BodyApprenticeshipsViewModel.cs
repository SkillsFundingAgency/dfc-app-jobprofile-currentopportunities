﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class BodyApprenticeshipsViewModel
    {
        [Display(Name = "Apprenticeships")]
        public IEnumerable<BodyVacancyViewModel> Vacancies { get; set; }
    }
}
