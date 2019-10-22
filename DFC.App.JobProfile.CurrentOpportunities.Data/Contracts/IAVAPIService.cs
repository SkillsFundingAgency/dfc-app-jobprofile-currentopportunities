using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface IAVAPIService
    {
        Task<ApprenticeshipVacancyDetails> GetApprenticeshipVacancyDetailsAsync(int vacancyRef);

        Task<IEnumerable<ApprenticeshipVacancySummary>> GetAVsForMultipleProvidersAsync(AVMapping mapping);

        Task<ApprenticeshipVacancySummaryResponse> GetAVSumaryPageAsync(AVMapping mapping, int pageNumber);

    }
}
