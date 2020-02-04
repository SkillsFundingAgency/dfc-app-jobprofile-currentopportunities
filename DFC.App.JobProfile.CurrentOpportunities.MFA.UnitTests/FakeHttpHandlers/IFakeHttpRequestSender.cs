using System.Net.Http;

namespace DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests.FakeHttpHandlers
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}