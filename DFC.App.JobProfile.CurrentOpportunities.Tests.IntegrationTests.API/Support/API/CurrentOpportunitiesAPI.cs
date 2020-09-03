using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.Support;
using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API.RestFactory.Interface;
using RestSharp;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API
{
    public class CurrentOpportunitiesAPI : ICurrentOpportunitiesAPI
    {
        private readonly IRestClientFactory restClientFactory;
        private readonly IRestRequestFactory restRequestFactory;
        private readonly AppSettings appSettings;

        public CurrentOpportunitiesAPI(IRestClientFactory restClientFactory, IRestRequestFactory restRequestFactory, AppSettings appSettings)
        {
            this.restClientFactory = restClientFactory;
            this.restRequestFactory = restRequestFactory;
            this.appSettings = appSettings;
        }

        public async Task<IRestResponse<T>> GetById<T>(string id)
            where T : class, new()
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var restClient = this.restClientFactory.Create(this.appSettings.APIConfig.EndpointBaseUrl);
            var restRequest = this.restRequestFactory.Create($"/segment/{id}/contents");
            restRequest.AddHeader("Accept", "application/json");
            return await Task.Run(() => restClient.Execute<T>(restRequest)).ConfigureAwait(false);
        }
    }
}
