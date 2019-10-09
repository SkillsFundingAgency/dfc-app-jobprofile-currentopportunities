﻿using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService
{
    public class AVAPIService : IAVAPIService
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

        public async Task<ApprenticeshipVacancyDetails> GetApprenticeshipVacancyDetailsAsync(int vacancyRef)
        {
            var responseResult = await apprenticeshipVacancyApi.GetAsync($"{vacancyRef}", RequestType.Apprenticeships).ConfigureAwait(true);
            logger.LogInformation($"Got details for vacancy ref : {vacancyRef}");
            return JsonConvert.DeserializeObject<ApprenticeshipVacancyDetails>(responseResult);
        }

        public async Task<IEnumerable<ApprenticeshipVacancySummary>> GetAVsForMultipleProvidersAsync(AVMapping mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            List<ApprenticeshipVacancySummary> avSummary = new List<ApprenticeshipVacancySummary>();

            var pageNumber = 0;

            logger.LogInformation($"Getting vacancies for mapping {JsonConvert.SerializeObject(mapping)}");

            //Allways break after a given number off loops
            while (aVAPIServiceSettings.FAAMaxPagesToTryPerMapping > pageNumber)
            {
                var apprenticeshipVacancySummaryResponse = await GetAVSumaryPageAsync(mapping, ++pageNumber).ConfigureAwait(false);

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

        public async Task<ApprenticeshipVacancySummaryResponse> GetAVSumaryPageAsync(AVMapping mapping, int pageNumber)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            logger.LogInformation($"Extracting AV summaries for Standards = {mapping.Frameworks} Frameworks = {mapping.Standards} page : {pageNumber}");

            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (mapping.Standards != null)
            {
                queryString["standardLarsCodes"] = string.Join(",", mapping.Standards.Where(s => !string.IsNullOrEmpty(s)));
            }

            if (mapping.Frameworks != null)
            {
                queryString["frameworkLarsCodes"] = string.Join(",", mapping.Frameworks.Where(s => !string.IsNullOrEmpty(s)));
            }

            queryString["pageSize"] = $"{aVAPIServiceSettings.FAAPageSize}";
            queryString["pageNumber"] = $"{pageNumber}";
            queryString["sortBy"] = aVAPIServiceSettings.FAASortBy;

            var responseResult = await apprenticeshipVacancyApi.GetAsync(queryString.ToString(), RequestType.Search).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<ApprenticeshipVacancySummaryResponse>(responseResult);
        }
    }
}
