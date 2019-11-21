using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class ApprenticeshipVacancySummaryResponse
    {
        public int TotalMatched { get; set; }

        public int TotalReturned { get; set; }

        public int CurrentPage { get; set; }

        public double TotalPages { get; set; }

        public string SortBy { get; set; }

        public IEnumerable<ApprenticeshipVacancySummary> Results { get; set; }
    }
}
