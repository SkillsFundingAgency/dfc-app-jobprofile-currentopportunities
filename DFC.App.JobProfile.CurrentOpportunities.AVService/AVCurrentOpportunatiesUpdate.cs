using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService
{
    public class AVCurrentOpportunatiesUpdate : IAVCurrentOpportunatiesUpdate
    {
        private readonly ILogger<AVCurrentOpportunatiesUpdate> logger;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly IAVAPIService aVAPIService;

        public AVCurrentOpportunatiesUpdate(ILogger<AVCurrentOpportunatiesUpdate> logger, ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService, IAVAPIService aVAPIService)
        {
            this.logger = logger;
            this.currentOpportunitiesSegmentService = currentOpportunitiesSegmentService;
            this.aVAPIService = aVAPIService;
        }

        public async Task<bool> UpdateApprenticeshipVacanciesAsync(string article)
        {
            logger.LogInformation($"{nameof(UpdateApprenticeshipVacanciesAsync)} has been called for article {article}");

            var currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentService.GetByNameAsync(article, false).ConfigureAwait(false);

            var aVMapping = new AVMapping() { Standards = currentOpportunitiesSegmentModel.Data.Standards , Frameworks = currentOpportunitiesSegmentModel.Data.Frameworks };
            var mappedVacancies = await aVAPIService.GetAVsForMultipleProvidersAsync(aVMapping).ConfigureAwait(false);

            var apprenticeships = Enumerable.Empty<Apprenticeship>();

            foreach (var vacancy in mappedVacancies)
            {
                var projectedVacancyDetails = await aVAPIService.GetApprenticeshipVacancyDetailsAsync(vacancy.VacancyReference).ConfigureAwait(false);

            }

        

            //Read article
            //Get vacancies
            //Project vacancies
            //get details
            //update reacord
            return true;
        }

        private IEnumerable<ApprenticeshipVacancySummary> ProjectVacanciesForSOC(IEnumerable<ApprenticeshipVacancySummary> mappedVacancies)
        {

            var projectedVacancies = Enumerable.Empty<ApprenticeshipVacancySummary>();

            //if none were found for SOC
            if (mappedVacancies == null)
            {
                return projectedVacancies;
            }

            var numberProvoidersFound = mappedVacancies.Select(v => v.TrainingProviderName).Distinct().Count();

            var projection = Enumerable.Empty<ApprenticeshipVacancySummary>();
            if (numberProvoidersFound > 1)
            {
                //have multipe providers
                projection = mappedVacancies
                    .GroupBy(v => v.TrainingProviderName)
                    .Select(g => g.First())
                    .Take(2);
            }
            else
            {
                //just have a single or no provider 
                projection = mappedVacancies.Take(2);
            }

            return projection;
        }
    }
}
