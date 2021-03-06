﻿using DFC.App.JobProfile.CurrentOpportunities.Data.HttpClientPolicies;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Models;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests.FakeHttpHandlers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests.Services.RefreshServiceTests
{
    [Trait("Message Function App", "Get Simple List Tests")]
    public class RefreshServiceGetSimpleListTests
    {
        private readonly ILogger<RefreshService> fakeLogger;
        private readonly RefreshClientOptions fakeRefreshClientOptions;

        public RefreshServiceGetSimpleListTests()
        {
            fakeLogger = A.Fake<ILogger<RefreshService>>();
            fakeRefreshClientOptions = A.Fake<RefreshClientOptions>();

            fakeRefreshClientOptions.BaseAddress = new Uri("https://nowhere.com");
            fakeRefreshClientOptions.Timeout = new TimeSpan(0, 0, 5);
        }

        [Fact]
        public async Task GetSimpleListReturnsSuccessWhenOK()
        {
            // arrange
            const HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            var expectedResults = A.CollectionOfFake<SimpleJobProfileModel>(2);
            var httpResponse = new HttpResponseMessage { StatusCode = expectedStatusCode, Content = new StringContent(JsonConvert.SerializeObject(expectedResults)) };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);

            using (var httpClient = new HttpClient(fakeHttpMessageHandler))
            {
                var refreshService = new RefreshService(httpClient, fakeLogger, fakeRefreshClientOptions);

                A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

                // act
                var results = await refreshService.GetListAsync().ConfigureAwait(false);

                // assert
                Assert.Equal(expectedResults.Count, results.Count);
            }

            httpResponse.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task GetSimpleListReturnsNullWhenError()
        {
            // arrange
            const HttpStatusCode expectedStatusCode = HttpStatusCode.NotFound;
            IEnumerable<SimpleJobProfileModel> expectedResults = null;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedStatusCode, Content = new StringContent(JsonConvert.SerializeObject(expectedResults)) };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);

            using (var httpClient = new HttpClient(fakeHttpMessageHandler))
            {
                var refreshService = new RefreshService(httpClient, fakeLogger, fakeRefreshClientOptions);

                A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

                // act
                var results = await refreshService.GetListAsync().ConfigureAwait(false);

                // assert
                Assert.Equal(expectedResults, results);
            }

            httpResponse.Dispose();
            fakeHttpMessageHandler.Dispose();
        }
    }
}