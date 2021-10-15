using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API.RestFactory.Interface;
using RestSharp;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API.RestFactory
{
    public class RestRequestFactory : IRestRequestFactory
    {
        public IRestRequest Create(string urlSuffix = null)
        {
            return new RestRequest(urlSuffix);
        }
    }
}
