using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
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

        public async Task<CurrentOpportunitiesSegmentModel> CreateAsync(CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel)
        {
            if (currentOpportunitiesSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(currentOpportunitiesSegmentModel));
            }

            if (currentOpportunitiesSegmentModel.Data == null)
            {
                currentOpportunitiesSegmentModel.Data = new CurrentOpportunitiesSegmentData();
            }

            currentOpportunitiesSegmentModel.Updated = DateTime.UtcNow;

            var result = await repository.CreateAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            return result == HttpStatusCode.Created
                ? await GetByIdAsync(currentOpportunitiesSegmentModel.DocumentId).ConfigureAwait(false)
                : null;
        }

        public Task<bool> DeleteAsync(Guid documentId, int partitionKey)
        {
            throw new NotImplementedException();
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

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
        }

        public async Task<CurrentOpportunitiesSegmentModel> ReplaceAsync(CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel)
        {
            if (currentOpportunitiesSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(currentOpportunitiesSegmentModel));
            }

            if (currentOpportunitiesSegmentModel.Data == null)
            {
                currentOpportunitiesSegmentModel.Data = new CurrentOpportunitiesSegmentData();
            }

            currentOpportunitiesSegmentModel.Updated = DateTime.UtcNow;

            var result = await repository.UpdateAsync(currentOpportunitiesSegmentModel.DocumentId, currentOpportunitiesSegmentModel).ConfigureAwait(false);

            return result == HttpStatusCode.OK
                ? await GetByIdAsync(currentOpportunitiesSegmentModel.DocumentId).ConfigureAwait(false)
                : null;
        }
    }
}