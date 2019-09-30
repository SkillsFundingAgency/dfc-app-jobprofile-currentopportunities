using DDFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.Integration.AVFeed.Data.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService
{
    public class AVAPIService : IAVService
    {
        private readonly IApprenticeshipVacancyApi apprenticeshipVacancyApi;
        private readonly ILogger<AVAPIService> logger;
        private readonly AVAPIServiceSettings aVAPIServiceSettings;

        public AVAPIService(IApprenticeshipVacancyApi apprenticeshipVacancyApi, ILogger<AVAPIService> logger, AVAPIServiceSettings aVAPIServiceSettings)
        {
            this.apprenticeshipVacancyApi = apprenticeshipVacancyApi;
            this.logger = logger;
            this.aVAPIServiceSettings = aVAPIServiceSettings;
        }


        public async Task<ApprenticeshipVacancyDetails> GetApprenticeshipVacancyDetailsAsync(string vacancyRef)
        {
            if (vacancyRef == null)
            {
                throw new ArgumentNullException(nameof(vacancyRef));
            }

            var responseResult = await apprenticeshipVacancyApi.GetAsync($"{vacancyRef}", RequestType.apprenticeships).ConfigureAwait(true);
            logger.LogInformation($"Got details for vacancy ref : {vacancyRef}");
            return JsonConvert.DeserializeObject<ApprenticeshipVacancyDetails>(responseResult);
        }

        public async Task<IEnumerable<ApprenticeshipVacancySummary>> GetAVsForMultipleProvidersAsync(SocMapping mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(SocMapping));
            }

            List<ApprenticeshipVacancySummary> avSummary = new List<ApprenticeshipVacancySummary>();

            var pageNumber = 0;
  
            logger.LogInformation($"Getting vacancies for mapping {JsonConvert.SerializeObject(mapping)}");

            //Allways break after a given number off loops
            while (aVAPIServiceSettings.FAAMaxPagesToTryPerMapping > pageNumber)
            {
                var apprenticeshipVacancySummaryResponse = await GetAVSumaryPageAsync(mapping, ++pageNumber);

                logger.LogInformation($"Got {apprenticeshipVacancySummaryResponse.TotalReturned} vacancies of {apprenticeshipVacancySummaryResponse.TotalMatched} on page: {pageNumber} of {apprenticeshipVacancySummaryResponse.TotalPages}");

                avSummary.AddRange(apprenticeshipVacancySummaryResponse.Results);

                //stop when there are no more pages or we have more then multiple supplier
                if (apprenticeshipVacancySummaryResponse.TotalPages < pageNumber ||
                     avSummary.Select(v => v.TrainingProviderName).Distinct().Count() > 1)
                {
                    break;
                }
            }

            return avSummary;
        }

        public async Task<ApprenticeshipVacancySummaryResponse> GetAVSumaryPageAsync(SocMapping mapping, int pageNumber)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(SocMapping));
            }

            logger.LogInformation($"Extracting AV summaries for SOC {mapping.SocCode} page : {pageNumber}");

            var queryString = HttpUtility.ParseQueryString(string.Empty);

            queryString["standardLarsCodes"] = string.Join(",", mapping.Standards);
            queryString["frameworkLarsCodes"] = string.Join(",", mapping.Frameworks);
            queryString["pageSize"] = $"{aVAPIServiceSettings.FAAPageSize}";
            queryString["pageNumber"] = $"{pageNumber}";
            queryString["sortBy"] = aVAPIServiceSettings.FAASortBy;

            var responseResult = await apprenticeshipVacancyApi.GetAsync(queryString.ToString(), RequestType.search).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<ApprenticeshipVacancySummaryResponse>(responseResult);
        }

        Task<ApprenticeshipVacancySummaryResponse> IAVService.GetAVSumaryPageAsync(SocMapping mapping, int pageNumber)
        {
            throw new NotImplementedException();
        }
    }
}
