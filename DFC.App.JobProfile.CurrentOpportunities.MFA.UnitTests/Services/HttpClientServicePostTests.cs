using DFC.App.JobProfile.CurrentOpportunities.Data.HttpClientPolicies;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests.FakeHttpHandlers;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests.Services
{
    [Trait("Messaging Function", "HttpClientService Post Tests")]
    public class HttpClientServicePostTests
    {
        private Mock<IHttpClientFactory> mockFactory;
        private readonly ILogService logService;
        private readonly ICorrelationIdProvider correlationIdProvider;
        private readonly CoreClientOptions coreClientOptions;

        public HttpClientServicePostTests()
        {
            logService = A.Fake<ILogService>();
            correlationIdProvider = A.Fake<ICorrelationIdProvider>();
            coreClientOptions = new CoreClientOptions
            {
                BaseAddress = new Uri("https://somewhere.com", UriKind.Absolute),
            };
        }

        public Mock<IHttpClientFactory> CreateClientFactory(HttpClient httpClient)
        {
            mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            return mockFactory;
        }

        [Fact]
        public async Task PostFullJobProfileAsyncReturnsOkStatusCodeForExistingId()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = coreClientOptions.BaseAddress };
            var httpClientService = new HttpClientService(coreClientOptions, CreateClientFactory(httpClient).Object, logService, correlationIdProvider);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var result = await httpClientService.PostAsync(A.Fake<CurrentOpportunitiesSegmentModel>()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task PostFullJobProfileAsyncReturnsExceptionForBadStatus()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.Forbidden;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult, Content = new StringContent("bad Post") };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = coreClientOptions.BaseAddress };
            var httpClientService = new HttpClientService(coreClientOptions, CreateClientFactory(httpClient).Object, logService, correlationIdProvider);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var exceptionResult = await Assert.ThrowsAsync<HttpRequestException>(async () => await httpClientService.PostAsync(A.Fake<CurrentOpportunitiesSegmentModel>()).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal($"Response status code does not indicate success: {(int)expectedResult} ({expectedResult}).", exceptionResult.Message);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }
    }
}
