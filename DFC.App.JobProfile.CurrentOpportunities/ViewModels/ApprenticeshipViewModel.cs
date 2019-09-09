using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class ApprenticeshipViewModel
    {
        public string Title { get; set; }

        public Uri Uri { get; set; }

        [Display(Name = "Wage")]
        public string Wage { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }
    }
}
