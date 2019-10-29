using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public class CurrentOpportunitiesSegmentService : ICurrentOpportunitiesSegmentService, IHealthCheck
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public CurrentOpportunitiesSegmentService(ICosmosRepository<CurrentOpportunitiesSegmentModel> repository, IMapper mapper, IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService)
        {
            this.repository = repository;
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

            var result = await repository.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            return result;
        }

        public async Task<HttpStatusCode> PatchSocCodeDataAsync(PatchSocDataModel patchModel, Guid documentId)
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

            var existingSocData = existingSegmentModel.Data.Soc;
            if (existingSocData is null)
            {
                return patchModel.MessageAction == MessageAction.Deleted ? HttpStatusCode.AlreadyReported : HttpStatusCode.NotFound;
            }

            if (patchModel.MessageAction == MessageAction.Deleted) // What should this do on delete of SocData - null or new SocData?
            {
                existingSegmentModel.Data.Soc = new SocData();
            }
            else
            {
                var updatedSocData = mapper.Map<SocData>(patchModel);
                existingSegmentModel.Data.Soc = updatedSocData;
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

            if (result == HttpStatusCode.OK || result == HttpStatusCode.Created)
            {
                var refreshJobProfileSegmentServiceBusModel = mapper.Map<RefreshJobProfileSegmentServiceBusModel>(existingSegmentModel);

                await jobProfileSegmentRefreshService.SendMessageAsync(refreshJobProfileSegmentServiceBusModel).ConfigureAwait(false);
            }

            return result;
        }
    }
}