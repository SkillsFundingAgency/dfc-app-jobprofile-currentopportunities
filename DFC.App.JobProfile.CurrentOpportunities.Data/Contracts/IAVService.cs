using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.Integration.AVFeed.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface IAVService
    {
        Task<ApprenticeshipVacancyDetails> GetApprenticeshipVacancyDetailsAsync(string vacancyRef);

        Task<IEnumerable<ApprenticeshipVacancySummary>> GetAVsForMultipleProvidersAsync(SocMapping mapping);

        Task<ApprenticeshipVacancySummaryResponse> GetAVSumaryPageAsync(SocMapping mapping, int pageNumber);
    }


}
