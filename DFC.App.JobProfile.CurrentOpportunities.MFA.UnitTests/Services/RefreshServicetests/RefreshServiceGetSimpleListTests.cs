using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Models;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
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
            var expectedResults = A.CollectionOfFake<SimpleJobProfileModel>(2);
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(JsonConvert.SerializeObject(expectedResults), HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var refreshService = new RefreshService(httpClient, fakeLogger, fakeRefreshClientOptions);

                    // act
                    var results = await refreshService.GetListAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResults);
                }
            }
        }

        [Fact]
        public async Task GetSimpleListReturnsNullWhenError()
        {
            // arrange
            IEnumerable<SimpleJobProfileModel> expectedResults = null;
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(JsonConvert.SerializeObject(expectedResults), HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var refreshService = new RefreshService(httpClient, fakeLogger, fakeRefreshClientOptions);

                    // act
                    var results = await refreshService.GetListAsync().ConfigureAwait(false);

                    // assert
                    A.Equals(results, expectedResults);
                }
            }
        }
    }
}
