using RestSharp;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API
{
    public interface ICurrentOpportunitiesAPI
    {
        Task<IRestResponse<T>> GetById<T>(string id)
            where T : class, new();
    }
}
