using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface IAVCurrentOpportuntiesRefresh
    {
        Task<int> RefreshApprenticeshipVacanciesAsync(Guid documentId);

        IEnumerable<ApprenticeshipVacancySummary> ProjectVacanciesForProfile(IEnumerable<ApprenticeshipVacancySummary> mappedVacancies);
    }
}
