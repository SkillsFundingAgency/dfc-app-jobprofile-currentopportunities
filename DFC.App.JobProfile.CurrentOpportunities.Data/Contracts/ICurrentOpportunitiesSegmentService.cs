using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface ICurrentOpportunitiesSegmentService
    {
        Task<bool> PingAsync();

        Task<IEnumerable<CurrentOpportunitiesSegmentModel>> GetAllAsync();

        Task<CurrentOpportunitiesSegmentModel> GetByIdAsync(Guid documentId);

        Task<CurrentOpportunitiesSegmentModel> GetByNameAsync(string canonicalName, bool isDraft = false);

        Task<CurrentOpportunitiesSegmentModel> CreateAsync(CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel);

        Task<CurrentOpportunitiesSegmentModel> ReplaceAsync(CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel);

        Task<bool> DeleteAsync(Guid documentId, int partitionKey);
    }
}
