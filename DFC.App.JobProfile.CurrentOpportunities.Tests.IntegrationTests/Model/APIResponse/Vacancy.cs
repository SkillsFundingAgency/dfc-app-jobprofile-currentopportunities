using System;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.APIResponse
{
    public class Vacancy
    {
        public string title { get; set; }

        public string apprenticeshipId { get; set; }

        public string wageUnit { get; set; }

        public string wageText { get; set; }

        public VacancyLocation location { get; set; }

        public string url { get; set; }

        public DateTime pullDate { get; set; }
    }
}
