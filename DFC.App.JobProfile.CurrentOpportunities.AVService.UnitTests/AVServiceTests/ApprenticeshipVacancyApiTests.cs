using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService.UnitTests
{
    public class ApprenticeshipVacancyApiTests
    {
        private ILogger<ApprenticeshipVacancyApi> fakeLogger;
        private AVAPIServiceSettings aVAPIServiceSettings;
        private ICosmosRepository<APIAuditRecord> fakeAuditRepository;

        public ApprenticeshipVacancyApiTests()
        {
            fakeLogger = A.Fake<ILogger<ApprenticeshipVacancyApi>>();
            fakeAuditRepository = A.Fake<ICosmosRepository<APIAuditRecord>>();
            aVAPIServiceSettings = new AVAPIServiceSettings() { FAAMaxPagesToTryPerMapping = 100, FAAEndPoint = "https://doesnotgoanywhere.com", RequestTimeOutSeconds = 10 };
        }

        [Fact]
        public async Task GetAsyncTestAsync()
        {
            var expectedResponseJson = $"ExpectedResponse";

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(expectedResponseJson, HttpStatusCode.OK))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    //arrange
                    var apprenticeshipVacancyApi = new ApprenticeshipVacancyApi(fakeLogger, fakeAuditRepository, aVAPIServiceSettings, httpClient);

                    //Act
                    var result = await apprenticeshipVacancyApi.GetAsync("fakeRequest", RequestType.Search).ConfigureAwait(false);

                    //Asserts
                    A.CallTo(() => fakeAuditRepository.UpsertAsync(A<APIAuditRecord>.Ignored)).MustHaveHappened();

                    result.Should().Be("ExpectedResponse");
                }
            }
        }

        [Fact]
        public void GetAsyncWithErrorTest()
        {
            var expectedResponseJson = $"ExpectedResponse";

            using (var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(expectedResponseJson, HttpStatusCode.BadRequest))
            {
                using (var httpClient = new HttpClient(messageHandler))
                {
                    //arrange
                    var apprenticeshipVacancyApi = new ApprenticeshipVacancyApi(fakeLogger, fakeAuditRepository, aVAPIServiceSettings, httpClient);

                    Func<Task> f = async () => { await apprenticeshipVacancyApi.GetAsync("fakeRequest", RequestType.Search).ConfigureAwait(false); };
                    f.Should().Throw<HttpRequestException>();

                    //Asserts
                    A.CallTo(() => fakeAuditRepository.UpsertAsync(A<APIAuditRecord>.Ignored)).MustHaveHappened();
                }
            }
        }
    }
}
