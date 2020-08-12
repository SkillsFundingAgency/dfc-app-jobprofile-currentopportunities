using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using DFC.App.JobProfile.CurrentOpportunities.SegmentService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService
{
    public class AVCurrentOpportuntiesRefresh : IAVCurrentOpportunitiesRefresh
    {
        private readonly ILogger<AVCurrentOpportuntiesRefresh> logger;
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly IAVAPIService aVAPIService;
        private readonly AutoMapper.IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public AVCurrentOpportuntiesRefresh(ILogger<AVCurrentOpportuntiesRefresh> logger, ICosmosRepository<CurrentOpportunitiesSegmentModel> repository, IAVAPIService aVAPIService, AutoMapper.IMapper mapper, IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService)
        {
            this.logger = logger;
            this.repository = repository;
            this.aVAPIService = aVAPIService;
            this.mapper = mapper;
            this.jobProfileSegmentRefreshService = jobProfileSegmentRefreshService;
        }

        public async Task<int> RefreshApprenticeshipVacanciesAsync(Guid documentId)
        {
            logger.LogInformation($"{nameof(RefreshApprenticeshipVacanciesAsync)} has been called for document {documentId}");
            CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel = await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
            return await RefreshApprenticeshipVacanciesAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);
        }

        public async Task<int> RefreshApprenticeshipVacanciesAndUpdateJobProfileAsync(Guid documentId)
        {
            logger.LogInformation($"{nameof(RefreshApprenticeshipVacanciesAsync)} has been called for document {documentId}");
            CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel = await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
            var numberPulled = await RefreshApprenticeshipVacanciesAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);
            if (numberPulled > 0)
            {
                var refreshJobProfileSegmentServiceBusModel = mapper.Map<RefreshJobProfileSegmentServiceBusModel>(currentOpportunitiesSegmentModel);
                await jobProfileSegmentRefreshService.SendMessageAsync(refreshJobProfileSegmentServiceBusModel).ConfigureAwait(false);
            }

            return numberPulled;
        }

        private async Task<int> RefreshApprenticeshipVacanciesAsync(CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel)
        {
            var aVMapping = new AVMapping()
            {
                Standards = currentOpportunitiesSegmentModel.Data.Apprenticeships?.Standards?.Where(w => !string.IsNullOrWhiteSpace(w.Url)).Select(b => b.Url).ToArray(),
                Frameworks = currentOpportunitiesSegmentModel.Data.Apprenticeships?.Frameworks?.Where(w => !string.IsNullOrWhiteSpace(w.Url)).Select(b => b.Url).ToArray(),
            };

            var vacancies = new List<Vacancy>();
            if (aVMapping.Standards.Any() || aVMapping.Frameworks.Any())
            {
                var mappedVacancies = await aVAPIService.GetAVsForMultipleProvidersAsync(aVMapping).ConfigureAwait(false);
                var projectedVacancies = ProjectVacanciesForProfile(mappedVacancies);

                //projectedVacancies will contain at most 2 records
                foreach (var vacancy in projectedVacancies)
                {
                    var projectedVacancyDetails = await aVAPIService.GetApprenticeshipVacancyDetailsAsync(vacancy.VacancyReference).ConfigureAwait(false);
                    vacancies.Add(mapper.Map<Vacancy>(projectedVacancyDetails));
                    logger.LogInformation($"{nameof(RefreshApprenticeshipVacanciesAsync)} added details for {vacancy.VacancyReference} to list");
                }
            }
            else
            {
                logger.LogInformation($"{nameof(RefreshApprenticeshipVacanciesAsync)} no standards or frameworks found for document {currentOpportunitiesSegmentModel.DocumentId}, blanking vacancies");
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