using System;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class BodyVacancyViewModel
    {
        public string Title { get; set; }

        public string ApprenticeshipId { get; set; }

        public string WageUnit { get; set; }

        public string WageText { get; set; }

        public LocationViewModel Location { get; set; }

        public Uri URL { get; set; }
    }
}
