using DFC.App.JobProfile.CurrentOpportunities.Data.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService.UnitTests
{
    [Trait("AVAPIService", "Health Status Tests")]
    public class AVAPIServiceHealthStatusCheckTests
    {
        private ILogger<AVAPIService> fakeLogger;
        private AVAPIServiceSettings aVAPIServiceSettings;
        private IApprenticeshipVacancyApi fakeApprenticeshipVacancyApi;
        private HealthCheckContext dummyHealthCheckContext;

        public AVAPIServiceHealthStatusCheckTests()
        {
            fakeLogger = A.Fake<ILogger<AVAPIService>>();
            aVAPIServiceSettings = new AVAPIServiceSettings() { FAAMaxPagesToTryPerMapping = 100 };
            fakeApprenticeshipVacancyApi = A.Fake<IApprenticeshipVacancyApi>();
            aVAPIServiceSettings.StandardsForHealthCheck = A.Dummy<string>();
            dummyHealthCheckContext = A.Dummy<HealthCheckContext>();
        }

        [Theory]
        [InlineData(5, HealthStatus.Healthy)]
        [InlineData(0, HealthStatus.Degraded)]
        public async Task GetCurrentHealthStatusAsyncTestAsync(int recordsToReturn, HealthStatus expectedStatus)
        {
            //Arrange
            var pageNumber = 1;
            var pageSize = 5;
            var returnDiffrentProvidersOnPage = 1;
            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Search)).Returns(AVAPIDummyResponses.GetDummyApprenticeshipVacancySummaryResponse(pageNumber, pageSize, recordsToReturn, pageSize, returnDiffrentProvidersOnPage));
            aVAPIServiceSettings.StandardsForHealthCheck = A.Dummy<string>();

            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Act
            var serviceHealthStatus = await aVAPIService.CheckHealthAsync(dummyHealthCheckContext).ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.Status.Should().Be(expectedStatus);
            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Search)).MustHaveHappened();
        }

        [Fact]
        public void GetCurrentHealthStatusAsyncExceptionTestAsync()
        {
            //Arrange
            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Search)).Throws(new ApplicationException());
            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Act
            Func<Task> serviceHealthStatus = async () => await aVAPIService.CheckHealthAsync(dummyHealthCheckContext).ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.Should().Throw<Exception>();
        }
    }
}