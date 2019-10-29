using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public class CurrentOpportunitiesSegmentService : ICurrentOpportunitiesSegmentService, IHealthCheck
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly IDraftCurrentOpportunitiesSegmentService draftCurrentOpportunitiesSegmentService;
        private readonly ICourseCurrentOpportuntiesRefresh courseCurrentOpportuntiesRefresh;
        private readonly IAVCurrentOpportuntiesRefresh aVCurrentOpportunatiesRefresh;
        private readonly ILogger<CurrentOpportunitiesSegmentService> logger;

        public CurrentOpportunitiesSegmentService(ICosmosRepository<CurrentOpportunitiesSegmentModel> repository, IDraftCurrentOpportunitiesSegmentService draftCurrentOpportunitiesSegmentService, ICourseCurrentOpportuntiesRefresh courseCurrentOpportuntiesRefresh, IAVCurrentOpportuntiesRefresh aVCurrentOpportunatiesRefresh, ILogger<CurrentOpportunitiesSegmentService> logger)
        {
            this.repository = repository;
            this.draftCurrentOpportunitiesSegmentService = draftCurrentOpportunitiesSegmentService;
            this.aVCurrentOpportunatiesRefresh = aVCurrentOpportunatiesRefresh;
            this.courseCurrentOpportuntiesRefresh = courseCurrentOpportuntiesRefresh;
            this.logger = logger;
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

            return isDraft
                ? await draftCurrentOpportunitiesSegmentService.GetSitefinityData(canonicalName.ToLowerInvariant()).ConfigureAwait(false)
                : await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
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

            var result = await repository.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            if (result == HttpStatusCode.Created || result == HttpStatusCode.OK)
            {
                try
                {
                    var avResult = await aVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(currentOpportunitiesSegmentModel.DocumentId).ConfigureAwait(false);
                    var avCourse = await courseCurrentOpportuntiesRefresh.RefreshCoursesAsync(currentOpportunitiesSegmentModel.DocumentId).ConfigureAwait(false);
                }
                catch (HttpRequestException httpRequestException)
                {
                    logger.LogError($"{nameof(CurrentOpportunitiesSegmentService)} had exception when getting courses and apprenticeships for document {currentOpportunitiesSegmentModel.DocumentId}, Exception - {httpRequestException.Message}");
                    return HttpStatusCode.Accepted;
                }
            }

            return result;
        }

        public async Task<bool> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);

            return result == HttpStatusCode.NoContent;
        }
    }
}