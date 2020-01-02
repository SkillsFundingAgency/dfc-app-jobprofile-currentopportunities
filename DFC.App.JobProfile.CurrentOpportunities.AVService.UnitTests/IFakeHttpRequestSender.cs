using System.Net.Http;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService.UnitTests
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}