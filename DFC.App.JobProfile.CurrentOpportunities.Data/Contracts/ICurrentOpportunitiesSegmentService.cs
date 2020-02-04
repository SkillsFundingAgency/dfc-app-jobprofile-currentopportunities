using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface ICurrentOpportunitiesSegmentService
    {
        Task<bool> PingAsync();

        Task<IEnumerable<CurrentOpportunitiesSegmentModel>> GetAllAsync();

        Task<CurrentOpportunitiesSegmentModel> GetByIdAsync(Guid documentId);

        Task<CurrentOpportunitiesSegmentModel> GetByNameAsync(string canonicalName);

        Task<HttpStatusCode> UpsertAsync(CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel);

        Task<bool> DeleteAsync(Guid documentId);

        Task<HttpStatusCode> PatchJobProfileSocAsync(PatchJobProfileSocModel patchModel, Guid documentId);

        Task<HttpStatusCode> PatchApprenticeshipFrameworksAsync(PatchApprenticeshipFrameworksModel patchModel, Guid documentId);

        Task<HttpStatusCode> PatchApprenticeshipStandardsAsync(PatchApprenticeshipStandardsModel patchModel, Guid documentId);
    }
}
