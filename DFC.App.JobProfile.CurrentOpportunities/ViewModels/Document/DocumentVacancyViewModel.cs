using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    [ExcludeFromCodeCoverage]
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
