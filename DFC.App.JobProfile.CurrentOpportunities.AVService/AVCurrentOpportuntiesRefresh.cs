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
    public class AVCurrentOpportuntiesRefresh : IAVCurrentOpportuntiesRefresh
    {
        private readonly ILogger<AVCurrentOpportuntiesRefresh> logger;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly IAVAPIService aVAPIService;
        private readonly AutoMapper.IMapper mapper;

        public AVCurrentOpportuntiesRefresh(ILogger<AVCurrentOpportuntiesRefresh> logger, ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService, IAVAPIService aVAPIService, AutoMapper.IMapper mapper)
        {
            this.logger = logger;
            this.currentOpportunitiesSegmentService = currentOpportunitiesSegmentService;
            this.aVAPIService = aVAPIService;
            this.mapper = mapper;
        }

        public async Task<bool> RefreshApprenticeshipVacanciesAsync(string article)
        {
            logger.LogInformation($"{nameof(RefreshApprenticeshipVacanciesAsync)} has been called for article {article}");

            CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentService.GetByNameAsync(article, false).ConfigureAwait(false);

            var aVMapping = new AVMapping() { Standards = currentOpportunitiesSegmentModel.Data.Apprenticeships.Standards, Frameworks = currentOpportunitiesSegmentModel.Data.Apprenticeships.Frameworks };
            var mappedVacancies = await aVAPIService.GetAVsForMultipleProvidersAsync(aVMapping).ConfigureAwait(false);

            var projectedVacancies = ProjectVacanciesForSOC(mappedVacancies);
            var vacancies = new List<Vacancy>();

            //projectedVacancies will contain at most 2 records
            foreach (var vacancy in projectedVacancies)
            {
                var projectedVacancyDetails = await aVAPIService.GetApprenticeshipVacancyDetailsAsync(vacancy.VacancyReference).ConfigureAwait(false);
                vacancies.Add(mapper.Map<Vacancy>(projectedVacancyDetails));
                logger.LogInformation($"{nameof(RefreshApprenticeshipVacanciesAsync)} added details for {vacancy.VacancyReference} to list");
            }

            currentOpportunitiesSegmentModel.Data.Apprenticeships.Vacancies = vacancies;
            await currentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

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
