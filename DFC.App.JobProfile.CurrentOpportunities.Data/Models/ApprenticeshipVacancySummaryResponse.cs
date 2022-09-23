using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class ApprenticeshipVacancySummaryResponse
    {
        public int Total { get; set; }

        public int TotalFiltered { get; set; }

        public double TotalPages { get; set; }

        public IEnumerable<ApprenticeshipVacancySummary> Vacancies { get; set; }
    }
}
