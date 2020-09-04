using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API.RestFactory.Interface;
using RestSharp;
using System;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API.RestFactory
{
    public class RestClientFactory : IRestClientFactory
    {
        public IRestClient Create(Uri baseUrl)
        {
            return new RestClient(baseUrl);
        }

        public IRestClient Create(string baseUrl)
        {
            return new RestClient(baseUrl);
        }
    }
}
