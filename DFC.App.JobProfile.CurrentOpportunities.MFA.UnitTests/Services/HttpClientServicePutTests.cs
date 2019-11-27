using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests.FakeHttpHandlers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests.Services
{
    [Trait("Messaging Function", "HttpClientService Put Tests")]
    public class HttpClientServicePutTests
    {
        private readonly ILogger logger;
        private readonly SegmentClientOptions segmentClientOptions;

        public HttpClientServicePutTests()
        {
            logger = A.Fake<ILogger>();
            segmentClientOptions = new SegmentClientOptions
            {
                BaseAddress = new Uri("https://somewhere.com", UriKind.Absolute),
            };
        }

        [Fact]
        public async Task PutFullJobProfileAsyncReturnsOkStatusCodeForExistingId()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = segmentClientOptions.BaseAddress };
            var httpClientService = new HttpClientService(segmentClientOptions, httpClient, logger);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var result = await httpClientService.PutAsync(A.Fake<CurrentOpportunitiesSegmentModel>()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task PutFullJobProfileAsyncReturnsNotFoundStatusCodeForExistingId()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.NotFound;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = segmentClientOptions.BaseAddress };
            var httpClientService = new HttpClientService(segmentClientOptions, httpClient, logger);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var result = await httpClientService.PutAsync(A.Fake<CurrentOpportunitiesSegmentModel>()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task PutFullJobProfileAsyncReturnsExceptionForBadStatus()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.Forbidden;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult, Content = new StringContent("bad Put") };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = segmentClientOptions.BaseAddress };
            var httpClientService = new HttpClientService(segmentClientOptions, httpClient, logger);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var exceptionResult = await Assert.ThrowsAsync<HttpRequestException>(async () => await httpClientService.PutAsync(A.Fake<CurrentOpportunitiesSegmentModel>()).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal($"Response status code does not indicate success: {(int)expectedResult} ({expectedResult}).", exceptionResult.Message);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }
    }
}
