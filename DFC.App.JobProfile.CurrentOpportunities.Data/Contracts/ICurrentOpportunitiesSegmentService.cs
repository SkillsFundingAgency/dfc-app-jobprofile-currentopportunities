using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface ICurrentOpportunitiesSegmentService
    {
        Task<IEnumerable<CurrentOpportunitiesSegmentModel>> GetAllAsync();

        Task<CurrentOpportunitiesSegmentModel> GetByIdAsync(Guid documentId);

        Task<CurrentOpportunitiesSegmentModel> GetByNameAsync(string canonicalName, bool isDraft = false);
    }
}
