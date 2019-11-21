using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Models;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
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
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(null, expectedStatusCode))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var refreshService = new RefreshService(httpClient, fakeLogger, fakeRefreshClientOptions);

                    // act
                    var result = await refreshService.RefreshApprenticeshipsAsync(documentId).ConfigureAwait(false);

                    // assert
                    A.Equals(result, expectedStatusCode);
                }
            }
        }

        [Fact]
        public async Task RefreshApprenticeshipsReturnsNullWhenError()
        {
            // arrange
            var documentId = Guid.NewGuid();
            var expectedStatusCode = HttpStatusCode.BadRequest;
            var expectedResults = A.Fake<FeedRefreshResponseModel>();
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(JsonConvert.SerializeObject(expectedResults), expectedStatusCode))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var refreshService = new RefreshService(httpClient, fakeLogger, fakeRefreshClientOptions);

                    // act
                    var result = await refreshService.RefreshApprenticeshipsAsync(documentId).ConfigureAwait(false);

                    // assert
                    A.Equals(result, expectedStatusCode);
                }
            }
        }

        [Fact]
        public async Task RefreshApprenticeshipsReturnsNullWhenException()
        {
            // arrange
            var documentId = Guid.NewGuid();
            var expectedStatusCode = HttpStatusCode.InternalServerError;
            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(null, expectedStatusCode))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    var refreshService = new RefreshService(null, fakeLogger, fakeRefreshClientOptions);

                    // act
                    var result = await refreshService.RefreshApprenticeshipsAsync(documentId).ConfigureAwait(false);

                    // assert
                    A.Equals(result, expectedStatusCode);
                }
            }
        }
    }
}
