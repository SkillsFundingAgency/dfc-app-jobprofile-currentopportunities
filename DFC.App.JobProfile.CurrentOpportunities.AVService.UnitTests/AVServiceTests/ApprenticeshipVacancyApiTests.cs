using DFC.App.JobProfile.CurrentOpportunities.Data.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService.UnitTests
{
    [Trait("Apprenticeship Vacancy Api", "Tests")]
    public class ApprenticeshipVacancyApiTests
    {
        private readonly ILogger<ApprenticeshipVacancyApi> fakeLogger;
        private readonly AVAPIServiceSettings aVAPIServiceSettings;
        private readonly IAuditService auditService;

        public ApprenticeshipVacancyApiTests()
        {
            fakeLogger = A.Fake<ILogger<ApprenticeshipVacancyApi>>();
            auditService = A.Fake<IAuditService>();
            aVAPIServiceSettings = new AVAPIServiceSettings() { FAAMaxPagesToTryPerMapping = 100, FAAEndPoint = "https://doesnotgoanywhere.com", RequestTimeOutSeconds = 10 };
        }

        [Fact]
        public async Task GetAsyncTestAsync()
        {
            //arrange
            const string expectedResponse = "ExpectedResponse";

            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(expectedResponse) };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apprenticeshipVacancyApi = new ApprenticeshipVacancyApi(fakeLogger, auditService, aVAPIServiceSettings, httpClient);
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            //Act
            var result = await apprenticeshipVacancyApi.GetAsync("fakeRequest", RequestType.Search).ConfigureAwait(false);

            //Asserts
            A.CallTo(() => auditService.CreateAudit(A<object>.Ignored, A<object>.Ignored, A<Guid?>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResponse, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task GetAsyncWithErrorTest()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent("ExpectedResponse") };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apprenticeshipVacancyApi = new ApprenticeshipVacancyApi(fakeLogger, auditService, aVAPIServiceSettings, httpClient);
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(async () => await apprenticeshipVacancyApi.GetAsync("fakeRequest", RequestType.Search).ConfigureAwait(false)).ConfigureAwait(false);
            A.CallTo(() => auditService.CreateAudit(A<object>.Ignored, A<object>.Ignored, A<Guid?>.Ignored)).MustHaveHappenedOnceExactly();

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }
    }
}