using AutoMapper;
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
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public class CurrentOpportunitiesSegmentService : ICurrentOpportunitiesSegmentService, IHealthCheck
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly ICourseCurrentOpportunitiesRefresh courseCurrentOpportunitiesRefresh;
        private readonly IAVCurrentOpportunitiesRefresh aVCurrentOpportunatiesRefresh;
        private readonly ILogger<CurrentOpportunitiesSegmentService> logger;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;
        private readonly ICurrentOpportunitiesSegmentUtilities currentOpportunitiesSegmentUtilities;

        public CurrentOpportunitiesSegmentService(ICosmosRepository<CurrentOpportunitiesSegmentModel> repository, ICourseCurrentOpportunitiesRefresh courseCurrentOpportunitiesRefresh, IAVCurrentOpportunitiesRefresh aVCurrentOpportunatiesRefresh, ILogger<CurrentOpportunitiesSegmentService> logger, IMapper mapper, IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService, ICurrentOpportunitiesSegmentUtilities currentOpportunitiesSegmentUtilities)
        {
            this.repository = repository;
            this.aVCurrentOpportunatiesRefresh = aVCurrentOpportunatiesRefresh;
            this.courseCurrentOpportunitiesRefresh = courseCurrentOpportunitiesRefresh;
            this.logger = logger;
            this.mapper = mapper;
            this.jobProfileSegmentRefreshService = jobProfileSegmentRefreshService;
            this.currentOpportunitiesSegmentUtilities = currentOpportunitiesSegmentUtilities;
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

        public async Task<CurrentOpportunitiesSegmentModel> GetByNameAsync(string canonicalName)
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

            var currentOpportunitiesSegmentPatchStatus = currentOpportunitiesSegmentUtilities.IsSegementOkToPatch(existingSegmentModel, patchModel.SequenceNumber);
            if (!currentOpportunitiesSegmentPatchStatus.OkToPatch)
            {
                return currentOpportunitiesSegmentPatchStatus.ReturnStatusCode;
            }

            var existingApprenticeships = existingSegmentModel.Data.Apprenticeships;
            if (existingApprenticeships is null)
            {
                return currentOpportunitiesSegmentUtilities.GetReturnStatusForNullElementPatchRequest(patchModel.ActionType);
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
            var currentOpportunitiesSegmentPatchStatus = currentOpportunitiesSegmentUtilities.IsSegementOkToPatch(existingSegmentModel, patchModel.SequenceNumber);
            if (!currentOpportunitiesSegmentPatchStatus.OkToPatch)
            {
                return currentOpportunitiesSegmentPatchStatus.ReturnStatusCode;
            }

            if (existingSegmentModel.Data.Apprenticeships == null)
            {
                existingSegmentModel.Data.Apprenticeships = new Apprenticeships();
            }

            if (existingSegmentModel.Data.Apprenticeships.Frameworks == null)
            {
                existingSegmentModel.Data.Apprenticeships.Frameworks = new List<Data.Models.ApprenticeshipFramework>();
            }

            var existingApprenticeshipFrameworks = existingSegmentModel.Data?.Apprenticeships?.Frameworks?.FirstOrDefault(f => f.Id == patchModel.Id);

            if (existingApprenticeshipFrameworks is null)
            {
                return currentOpportunitiesSegmentUtilities.GetReturnStatusForNullElementPatchRequest(patchModel.ActionType);
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
            var currentOpportunitiesSegmentPatchStatus = currentOpportunitiesSegmentUtilities.IsSegementOkToPatch(existingSegmentModel, patchModel.SequenceNumber);
            if (!currentOpportunitiesSegmentPatchStatus.OkToPatch)
            {
                return currentOpportunitiesSegmentPatchStatus.ReturnStatusCode;
            }

            if (existingSegmentModel.Data.Apprenticeships == null)
            {
                existingSegmentModel.Data.Apprenticeships = new Apprenticeships();
            }

            if (existingSegmentModel.Data.Apprenticeships.Standards == null)
            {
                existingSegmentModel.Data.Apprenticeships.Standards = new List<Data.Models.ApprenticeshipStandard>();
            }

            var existingApprenticeshipStandards = existingSegmentModel.Data?.Apprenticeships?.Standards?.FirstOrDefault(f => f.Id == patchModel.Id);

            if (existingApprenticeshipStandards is null)
            {
                return currentOpportunitiesSegmentUtilities.GetReturnStatusForNullElementPatchRequest(patchModel.ActionType);
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
            int numberOfRefreshFailures = 0;
            if (result == HttpStatusCode.Created || result == HttpStatusCode.OK)
            {
                var refreshJobProfileSegmentServiceBusModel = mapper.Map<RefreshJobProfileSegmentServiceBusModel>(existingSegmentModel);
                numberOfRefreshFailures += await TryRefresh(() => aVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(existingSegmentModel.DocumentId), existingSegmentModel.CanonicalName).ConfigureAwait(false);
                numberOfRefreshFailures += await TryRefresh(() => courseCurrentOpportunitiesRefresh.RefreshCoursesAsync(existingSegmentModel.DocumentId), existingSegmentModel.CanonicalName).ConfigureAwait(false);
                if (numberOfRefreshFailures <= 1)
                {
                    await jobProfileSegmentRefreshService.SendMessageAsync(refreshJobProfileSegmentServiceBusModel).ConfigureAwait(false);
                }
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We want to catch all errors that happen when we call the external API")]
        private async Task<int> TryRefresh(Func<Task<int>> refreshMethod, string canonicalName)
        {
            try
            {
                await refreshMethod().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(refreshMethod)}: Error refreshing for {canonicalName}");
                return 1;
            }

            return 0;
        }
    }
}