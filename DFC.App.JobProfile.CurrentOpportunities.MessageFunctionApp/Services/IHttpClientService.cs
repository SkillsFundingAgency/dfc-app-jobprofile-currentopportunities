using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services
{
    public interface IHttpClientService
    {
        Task<HttpStatusCode> PostAsync(CurrentOpportunitiesSegmentModel overviewSegmentModel);

        Task<HttpStatusCode> PutAsync(CurrentOpportunitiesSegmentModel overviewSegmentModel);

        Task<HttpStatusCode> PatchAsync<T>(T patchModel, string patchTypeEndpoint)
            where T : BasePatchModel;

        Task<HttpStatusCode> DeleteAsync(Guid id);
    }
}