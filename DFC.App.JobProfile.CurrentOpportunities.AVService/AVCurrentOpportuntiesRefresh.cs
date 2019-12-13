using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService
{
    public class AVCurrentOpportuntiesRefresh : IAVCurrentOpportuntiesRefresh
    {
        private readonly ILogger<AVCurrentOpportuntiesRefresh> logger;
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly IAVAPIService aVAPIService;
        private readonly AutoMapper.IMapper mapper;

        public AVCurrentOpportuntiesRefresh(ILogger<AVCurrentOpportuntiesRefresh> logger, ICosmosRepository<CurrentOpportunitiesSegmentModel> repository, IAVAPIService aVAPIService, AutoMapper.IMapper mapper)
        {
            this.logger = logger;
            this.repository = repository;
            this.aVAPIService = aVAPIService;
            this.mapper = mapper;
        }

        public async Task<int> RefreshApprenticeshipVacanciesAsync(Guid documentId)
        {
            logger.LogInformation($"{nameof(RefreshApprenticeshipVacanciesAsync)} has been called for document {documentId}");

            CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel = await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
            var aVMapping = new AVMapping()
            {
                Standards = currentOpportunitiesSegmentModel.Data.Apprenticeships?.Standards?.Where(w => !string.IsNullOrWhiteSpace(w.Url)).Select(b => b.Url).ToArray(),
                Frameworks = currentOpportunitiesSegmentModel.Data.Apprenticeships?.Frameworks?.Where(w => !string.IsNullOrWhiteSpace(w.Url)).Select(b => b.Url).ToArray(),
            };
            var mappedVacancies = await aVAPIService.GetAVsForMultipleProvidersAsync(aVMapping).ConfigureAwait(false);

            var projectedVacancies = ProjectVacanciesForProfile(mappedVacancies);
            var vacancies = new List<Vacancy>();

            //projectedVacancies will contain at most 2 records
            foreach (var vacancy in projectedVacancies)
            {
                var projectedVacancyDetails = await aVAPIService.GetApprenticeshipVacancyDetailsAsync(vacancy.VacancyReference).ConfigureAwait(false);
                vacancies.Add(mapper.Map<Vacancy>(projectedVacancyDetails));
                logger.LogInformation($"{nameof(RefreshApprenticeshipVacanciesAsync)} added details for {vacancy.VacancyReference} to list");
            }

            currentOpportunitiesSegmentModel.Data.Apprenticeships.Vacancies = vacancies;
            await repository.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            return vacancies.Count;
        }

        public IEnumerable<ApprenticeshipVacancySummary> ProjectVacanciesForProfile(IEnumerable<ApprenticeshipVacancySummary> mappedVacancies)
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
