using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public class CurrentOpportunitiesSegmentService : ICurrentOpportunitiesSegmentService
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly IDraftCurrentOpportunitiesSegmentService draftCurrentOpportunitiesSegmentService;

        public CurrentOpportunitiesSegmentService(ICosmosRepository<CurrentOpportunitiesSegmentModel> repository, IDraftCurrentOpportunitiesSegmentService draftCurrentOpportunitiesSegmentService)
        {
            this.repository = repository;
            this.draftCurrentOpportunitiesSegmentService = draftCurrentOpportunitiesSegmentService;
        }

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
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

            return result;
        }

        public async Task<bool> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);

            return result == HttpStatusCode.NoContent;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We want to catch any type of error for this health check and report it")]
        public async Task<ServiceHealthStatus> GetCurrentHealthStatusAsync()
        {
            var serviceHealthStatus = new ServiceHealthStatus();
            serviceHealthStatus.Service = typeof(CurrentOpportunitiesSegmentService).Namespace;
            serviceHealthStatus.SubService = "Cosmos Document Store";
            serviceHealthStatus.HealthServiceState = HealthServiceState.Red;
            serviceHealthStatus.CheckParametersUsed = string.Empty;

            try
            {
                var isHealthy = await PingAsync().ConfigureAwait(false);

                if (isHealthy)
                {
                    serviceHealthStatus.Message = "Document store is available";
                    serviceHealthStatus.HealthServiceState = HealthServiceState.Green;
                }
                else
                {
                    serviceHealthStatus.Message = "Ping has failed";
                }
            }
            catch (Exception ex)
            {
                serviceHealthStatus.Message = $"Exception: {ex.Message}";
            }

            return serviceHealthStatus;
        }
    }
}