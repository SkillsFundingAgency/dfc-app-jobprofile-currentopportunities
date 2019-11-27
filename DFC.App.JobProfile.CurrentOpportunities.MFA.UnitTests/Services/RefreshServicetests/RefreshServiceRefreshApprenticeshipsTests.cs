using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Models;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests.FakeHttpHandlers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests.Services.RefreshServiceTests
{
    [Trait("Message Function App", "Refresh Apprenticeships Tests")]
    public class RefreshServiceRefreshApprenticeshipsTests
    {
        private readonly ILogger<RefreshService> fakeLogger;
        private readonly RefreshClientOptions fakeRefreshClientOptions;

        public RefreshServiceRefreshApprenticeshipsTests()
        {
            fakeLogger = A.Fake<ILogger<RefreshService>>();
            fakeRefreshClientOptions = A.Fake<RefreshClientOptions>();

            fakeRefreshClientOptions.BaseAddress = new Uri("https://nowhere.com");
            fakeRefreshClientOptions.Timeout = new TimeSpan(0, 0, 5);
        }

        [Fact]
        public async Task RefreshApprenticeshipsReturnsSuccessWhenOK()
        {
            // arrange
            var documentId = Guid.NewGuid();
            const HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedStatusCode };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            using (var httpClient = new HttpClient(fakeHttpMessageHandler))
            {
                var refreshService = new RefreshService(httpClient, fakeLogger, fakeRefreshClientOptions);

                A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

                // act
                var result = await refreshService.RefreshApprenticeshipsAsync(documentId).ConfigureAwait(false);

                // assert
                Assert.Equal(result, expectedStatusCode);
            }
        }

        [Fact]
        public async Task RefreshApprenticeshipsReturnsNotFound()
        {
            // arrange
            var documentId = Guid.NewGuid();
            const HttpStatusCode expectedStatusCode = HttpStatusCode.NotFound;
            var feedRefreshResponseModel = new FeedRefreshResponseModel() { NumberPulled = 0, RequestErrorMessage = "No results" };
            var httpResponse = new HttpResponseMessage { StatusCode = expectedStatusCode, Content = new StringContent(JsonConvert.SerializeObject(feedRefreshResponseModel)) };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            using (var httpClient = new HttpClient(fakeHttpMessageHandler))
            {
                var refreshService = new RefreshService(httpClient, fakeLogger, fakeRefreshClientOptions);

                A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

                // act
                var result = await refreshService.RefreshApprenticeshipsAsync(documentId).ConfigureAwait(false);

                // assert
                Assert.Equal(result, expectedStatusCode);
            }
        }

        [Fact]
        public async Task RefreshApprenticeshipsReturnsNullWhenError()
        {
            // arrange
            const HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
            var documentId = Guid.NewGuid();
            var expectedResults = A.Fake<FeedRefreshResponseModel>();
            var httpResponse = new HttpResponseMessage { StatusCode = expectedStatusCode };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            using (var httpClient = new HttpClient(fakeHttpMessageHandler))
            {
                var refreshService = new RefreshService(httpClient, fakeLogger, fakeRefreshClientOptions);

                A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

                // act
                var result = await refreshService.RefreshApprenticeshipsAsync(documentId).ConfigureAwait(false);

                // assert
                Assert.Equal(result, expectedStatusCode);
            }
        }

        [Fact]
        public async Task RefreshApprenticeshipsReturnsNullWhenException()
        {
            // arrange
            const HttpStatusCode expectedStatusCode = HttpStatusCode.InternalServerError;
            var documentId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage { StatusCode = expectedStatusCode };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            using (var httpClient = new HttpClient(fakeHttpMessageHandler))
            {
                var refreshService = new RefreshService(null, fakeLogger, fakeRefreshClientOptions);

                A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

                // act
                var result = await refreshService.RefreshApprenticeshipsAsync(documentId).ConfigureAwait(false);

                // assert
                Assert.Equal(result, expectedStatusCode);
            }
        }
    }
}
