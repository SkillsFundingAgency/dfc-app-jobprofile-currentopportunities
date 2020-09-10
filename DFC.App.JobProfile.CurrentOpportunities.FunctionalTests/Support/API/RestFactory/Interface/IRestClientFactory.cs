using RestSharp;
using System;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API.RestFactory.Interface
{
    public interface IRestClientFactory
    {
        IRestClient Create(Uri baseUrl);

        IRestClient Create(string baseUrl);
    }
}
