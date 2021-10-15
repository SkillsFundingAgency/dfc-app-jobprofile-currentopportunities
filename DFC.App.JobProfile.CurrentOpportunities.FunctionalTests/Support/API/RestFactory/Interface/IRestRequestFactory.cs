using RestSharp;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API.RestFactory.Interface
{
    public interface IRestRequestFactory
    {
        IRestRequest Create(string urlSuffix = null);
    }
}
