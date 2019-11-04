﻿using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public class CurrentOpportunitiesSegmentService : ICurrentOpportunitiesSegmentService, IHealthCheck
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly ICourseCurrentOpportuntiesRefresh courseCurrentOpportuntiesRefresh;
        private readonly IAVCurrentOpportuntiesRefresh aVCurrentOpportunatiesRefresh;
        private readonly ILogger<CurrentOpportunitiesSegmentService> logger;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public CurrentOpportunitiesSegmentService(ICosmosRepository<CurrentOpportunitiesSegmentModel> repository, ICourseCurrentOpportuntiesRefresh courseCurrentOpportuntiesRefresh, IAVCurrentOpportuntiesRefresh aVCurrentOpportunatiesRefresh, ILogger<CurrentOpportunitiesSegmentService> logger, IMapper mapper, IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService)
        {
            this.repository = repository;
            this.aVCurrentOpportunatiesRefresh = aVCurrentOpportunatiesRefresh;
            this.courseCurrentOpportuntiesRefresh = courseCurrentOpportuntiesRefresh;
            this.logger = logger;
            this.mapper = mapper;
            this.jobProfileSegmentRefreshService = jobProfileSegmentRefreshService;
        }

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var description = $"{typeof(CurrentOpportunitiesSegmentService).Namespace} - Cosmos Document Store";

            var isHealthy = await PingAsync().ConfigureAwait(false);
            if (isHealthy)
            {
                return HealthCheckResult.Healthy(description);
            }
            else
            {
                return HealthCheckResult.Degraded(description);
            }
        }

        public async Task<IEnumerable<CurrentOpportunitiesSegmentModel>> GetAllAsync()
        {
            return await repository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<CurrentOpportunitiesSegmentModel> GetByIdAsync(Guid documentId)
        {
            return await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
        }

        public async Task<CurrentOpportunitiesSegmentModel> GetByNameAsync(string canonicalName, bool isDraft = false)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            return await repository.GetAsync(d => d.CanonicalName.ToLower() == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> UpsertAsync(CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel)
        {
            if (currentOpportunitiesSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(currentOpportunitiesSegmentModel));
            }

            if (currentOpportunitiesSegmentModel.Data == null)
            {
                currentOpportunitiesSegmentModel.Data = new CurrentOpportunitiesSegmentDataModel();
            }

            return await UpsertAndRefreshSegmentModel(currentOpportunitiesSegmentModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchJobProfileSocAsync(PatchJobProfileSocModel patchModel, Guid documentId)
        {
            if (patchModel is null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            var existingSegmentModel = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingSegmentModel is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (patchModel.SequenceNumber <= existingSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var existingApprenticeships = existingSegmentModel.Data.Apprenticeships;
            if (existingApprenticeships is null)
            {
                return patchModel.ActionType == MessageAction.Deleted ? HttpStatusCode.AlreadyReported : HttpStatusCode.NotFound;
            }

            if (patchModel.ActionType == MessageAction.Deleted)
            {
                existingSegmentModel.Data.Apprenticeships = new Apprenticeships();
            }
            else
            {
                var updatedApprenticeships = new Data.Models.Apprenticeships()
                {
                    Frameworks = mapper.Map<IList<Data.Models.ApprenticeshipFramework>>(patchModel.ApprenticeshipFramework),
                    Standards = mapper.Map<IList<Data.Models.ApprenticeshipStandard>>(patchModel.ApprenticeshipStandards),
                    Vacancies = new List<Data.Models.Vacancy>(),
                };

                existingSegmentModel.Data.Apprenticeships = updatedApprenticeships;
            }

            existingSegmentModel.SequenceNumber = patchModel.SequenceNumber;

            return await UpsertAndRefreshSegmentModel(existingSegmentModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchApprenticeshipFrameworksAsync(PatchApprenticeshipFrameworksModel patchModel, Guid documentId)
        {
            if (patchModel is null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            var existingSegmentModel = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingSegmentModel is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (patchModel.SequenceNumber <= existingSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            if (existingSegmentModel.Data.Apprenticeships == null)
            {
                existingSegmentModel.Data.Apprenticeships = new Apprenticeships();
            }

            if (existingSegmentModel.Data.Apprenticeships.Frameworks == null)
            {
                existingSegmentModel.Data.Apprenticeships.Frameworks = new List<Data.Models.ApprenticeshipFramework>();
            }

            var existingApprenticeshipFrameworks = existingSegmentModel.Data.Apprenticeships.Frameworks.FirstOrDefault(f => f.Id == patchModel.Id);

            if (existingApprenticeshipFrameworks is null)
            {
                return patchModel.ActionType == MessageAction.Deleted ? HttpStatusCode.AlreadyReported : HttpStatusCode.NotFound;
            }

            if (patchModel.ActionType == MessageAction.Deleted)
            {
                existingSegmentModel.Data.Apprenticeships.Frameworks.Remove(existingApprenticeshipFrameworks);
            }
            else
            {
                mapper.Map(patchModel, existingApprenticeshipFrameworks);
            }

            existingSegmentModel.SequenceNumber = patchModel.SequenceNumber;

            return await UpsertAndRefreshSegmentModel(existingSegmentModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchApprenticeshipStandardsAsync(PatchApprenticeshipStandardsModel patchModel, Guid documentId)
        {
            if (patchModel is null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            var existingSegmentModel = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingSegmentModel is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (patchModel.SequenceNumber <= existingSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            if (existingSegmentModel.Data.Apprenticeships == null)
            {
                existingSegmentModel.Data.Apprenticeships = new Apprenticeships();
            }

            if (existingSegmentModel.Data.Apprenticeships.Standards == null)
            {
                existingSegmentModel.Data.Apprenticeships.Standards = new List<Data.Models.ApprenticeshipStandard>();
            }

            var existingApprenticeshipStandards = existingSegmentModel.Data.Apprenticeships.Standards.FirstOrDefault(f => f.Id == patchModel.Id);

            if (existingApprenticeshipStandards is null)
            {
                return patchModel.ActionType == MessageAction.Deleted ? HttpStatusCode.AlreadyReported : HttpStatusCode.NotFound;
            }

            if (patchModel.ActionType == MessageAction.Deleted)
            {
                existingSegmentModel.Data.Apprenticeships.Standards.Remove(existingApprenticeshipStandards);
            }
            else
            {
                mapper.Map(patchModel, existingApprenticeshipStandards);
            }

            existingSegmentModel.SequenceNumber = patchModel.SequenceNumber;

            return await UpsertAndRefreshSegmentModel(existingSegmentModel).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);

            return result == HttpStatusCode.NoContent;
        }

        private async Task<HttpStatusCode> UpsertAndRefreshSegmentModel(CurrentOpportunitiesSegmentModel existingSegmentModel)
        {
            var result = await repository.UpsertAsync(existingSegmentModel).ConfigureAwait(false);

            if (result == HttpStatusCode.Created || result == HttpStatusCode.OK)
            {
                try
                {
                    var avResult = await aVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(existingSegmentModel.DocumentId).ConfigureAwait(false);
                    var avCourse = await courseCurrentOpportuntiesRefresh.RefreshCoursesAsync(existingSegmentModel.DocumentId).ConfigureAwait(false);
                }
                catch (HttpRequestException httpRequestException)
                {
                    logger.LogError($"{nameof(UpsertAndRefreshSegmentModel)} had exception when getting courses and apprenticeships for document {existingSegmentModel.DocumentId}, Exception - {httpRequestException.Message}");
                    return HttpStatusCode.Accepted;
                }

                var refreshJobProfileSegmentServiceBusModel = mapper.Map<RefreshJobProfileSegmentServiceBusModel>(existingSegmentModel);

                await jobProfileSegmentRefreshService.SendMessageAsync(refreshJobProfileSegmentServiceBusModel).ConfigureAwait(false);
            }

            return result;
        }
    }
}