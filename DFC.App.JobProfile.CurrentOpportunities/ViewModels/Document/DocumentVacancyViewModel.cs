using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class DocumentVacancyViewModel
    {
        public string Title { get; set; }

        public string ApprenticeshipId { get; set; }

        public string WageUnit { get; set; }

        public string WageText { get; set; }

        public LocationViewModel Location { get; set; }

        public Uri URL { get; set; }

        [Display(Name = "Pull Date")]
        public DateTime PullDate { get; set; }
    }
}
