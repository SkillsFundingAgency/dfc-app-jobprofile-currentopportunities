using DFC.App.JobProfile.CurrentOpportunities.Data.HttpClientPolicies;
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
    [Trait("Message Function App", "Refresh Courses Tests")]
    public class RefreshServiceRefreshCoursesTests
    {
        private readonly ILogger<RefreshService> fakeLogger;
        private readonly RefreshClientOptions fakeRefreshClientOptions;

        public RefreshServiceRefreshCoursesTests()
        {
            fakeLogger = A.Fake<ILogger<RefreshService>>();
            fakeRefreshClientOptions = A.Fake<RefreshClientOptions>();

            fakeRefreshClientOptions.BaseAddress = new Uri("https://nowhere.com");
            fakeRefreshClientOptions.Timeout = new TimeSpan(0, 0, 5);
        }

        [Fact]
        public async Task RefreshCoursesReturnsSuccessWhenOK()
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
                var result = await refreshService.RefreshCoursesAsync(documentId).ConfigureAwait(false);

                // assert
                Assert.Equal(result, expectedStatusCode);
            }

            httpResponse.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task RefreshCoursesReturnsNotFound()
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
                var result = await refreshService.RefreshCoursesAsync(documentId).ConfigureAwait(false);

                // assert
                Assert.Equal(result, expectedStatusCode);
            }

            httpResponse.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task RefreshCoursesReturnsNullWhenError()
        {
            // arrange
            const HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
            var documentId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage { StatusCode = expectedStatusCode };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            using (var httpClient = new HttpClient(fakeHttpMessageHandler))
            {
                A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

                var refreshService = new RefreshService(httpClient, fakeLogger, fakeRefreshClientOptions);

                // act
                var result = await refreshService.RefreshCoursesAsync(documentId).ConfigureAwait(false);

                // assert
                Assert.Equal(result, expectedStatusCode);
            }

            httpResponse.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task RefreshCoursesReturnsNullWhenException()
        {
            // arrange
            const HttpStatusCode expectedStatusCode = HttpStatusCode.InternalServerError;
            var documentId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage { StatusCode = expectedStatusCode };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            using (var httpClient = new HttpClient(fakeHttpMessageHandler))
            {
                A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

                var refreshService = new RefreshService(null, fakeLogger, fakeRefreshClientOptions);

                // act
                var result = await refreshService.RefreshCoursesAsync(documentId).ConfigureAwait(false);

                // assert
                Assert.Equal(result, expectedStatusCode);
            }

            httpResponse.Dispose();
            fakeHttpMessageHandler.Dispose();
        }
    }
}