using DFC.App.JobProfile.CurrentOpportunities.Data.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using FluentAssertions;
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
        private AutoMapper.IMapper fakeMapper;
        private AVMapping aVMapping;

        public AVAPIServiceHealthStatusCheckTests()
        {
            fakeLogger = A.Fake<ILogger<AVAPIService>>();
            aVAPIServiceSettings = new AVAPIServiceSettings() { FAAMaxPagesToTryPerMapping = 100 };
            fakeApprenticeshipVacancyApi = A.Fake<IApprenticeshipVacancyApi>();
        }

        [Fact]
        public async Task GetCurrentHealthStatusAsyncTestAsync()
        {
            //Arrange
            var pageNumber = 1;
            var pageSize = 5;
            var returnDiffrentProvidersOnPage = 1;
            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Search)).Returns(AVAPIDummyResponses.GetDummyApprenticeshipVacancySummaryResponse(pageNumber, 50, pageSize, pageSize, returnDiffrentProvidersOnPage));

            aVAPIServiceSettings.StandardsForHealthCheck = A.Dummy<string>();

            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Act
            var serviceHealthStatus = await aVAPIService.GetCurrentHealthStatusAsync().ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.HealthServiceState.Should().Be(HealthServiceState.Green);
            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Search)).MustHaveHappened();
        }

        [Fact]
        public async Task GetCurrentHealthStatusAsyncExceptionTestAsync()
        {
            //Arrange
            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Search)).Throws(new ApplicationException());
            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Act
            var serviceHealthStatus = await aVAPIService.GetCurrentHealthStatusAsync().ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.HealthServiceState.Should().Be(HealthServiceState.Red);
        }
    }
}