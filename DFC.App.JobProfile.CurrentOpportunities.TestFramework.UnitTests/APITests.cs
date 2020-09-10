using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.APIResponse;
using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.Support;
using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API;
using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API.RestFactory.Interface;
using FakeItEasy;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.APITest.UnitTests
{
    public class APITests
    {
        private AppSettings appSettings;
        private IRestClientFactory restClientFactory;
        private IRestRequestFactory restRequestFactory;
        private CurrentOpportunitiesAPI currentOpportunitiesApi;
        private IRestClient restClient;
        private IRestRequest restRequest;

        public APITests()
        {
            this.appSettings = new AppSettings();
            this.restClientFactory = A.Fake<IRestClientFactory>();
            this.restRequestFactory = A.Fake<IRestRequestFactory>();
            this.restClient = A.Fake<IRestClient>();
            this.restRequest = A.Fake<IRestRequest>();
            A.CallTo(() => this.restClientFactory.Create(A<string>.Ignored)).Returns(this.restClient);
            A.CallTo(() => this.restRequestFactory.Create(A<string>.Ignored)).Returns(this.restRequest);
            this.currentOpportunitiesApi = new CurrentOpportunitiesAPI(this.restClientFactory, this.restRequestFactory, this.appSettings);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task EmptyOrNullIdResultsInNullBeingReturned(string id)
        {
            Assert.Null(await this.currentOpportunitiesApi.GetById(id).ConfigureAwait(true));
        }

        [Fact]
        public async Task SuccessfulGetRequest()
        {
            var apiResponse = new RestResponse<CurrentOpportunitiesAPIResponse>();
            apiResponse.StatusCode = HttpStatusCode.OK;
            A.CallTo(() => this.restClient.Execute<CurrentOpportunitiesAPIResponse>(this.restRequest)).Returns(apiResponse);
            var response = await this.currentOpportunitiesApi.GetById("id").ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("Accept", "application/json")]
        public async Task AllRequestHeadersAreSet(string headerKey, string headerValue)
        {
            var response = await this.currentOpportunitiesApi.GetById("id").ConfigureAwait(false);
            A.CallTo(() => this.restRequest.AddHeader(headerKey, headerValue)).MustHaveHappenedOnceExactly();
        }
    }
}
