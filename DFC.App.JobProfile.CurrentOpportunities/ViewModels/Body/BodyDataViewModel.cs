using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class BodyDataViewModel
    {
        public string JobTitle { get; set; }

        [Display(Name = "Apprenticeships")]
        public BodyApprenticeshipsViewModel Apprenticeships { get; set; }

        [Display(Name = "Courses")]
        public BodyCoursesViewModel Courses { get; set; }

    }
}
