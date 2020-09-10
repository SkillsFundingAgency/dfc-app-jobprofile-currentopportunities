using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.APIResponse;
using RestSharp;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API
{
    public interface ICurrentOpportunitiesAPI
    {
        Task<IRestResponse<CurrentOpportunitiesAPIResponse>> GetById(string id);
    }
}
