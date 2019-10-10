using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService.UnitTests
{
    public class AVAPIServiceTests
    {
        private ILogger<AVAPIService> fakeLogger;
        private AVAPIServiceSettings aVAPIServiceSettings;
        private IApprenticeshipVacancyApi fakeApprenticeshipVacancyApi;
        private AutoMapper.IMapper fakeMapper;
        private AVMapping aVMapping;

        public AVAPIServiceTests()
        {
            fakeLogger = A.Fake<ILogger<AVAPIService>>();
            aVAPIServiceSettings = new AVAPIServiceSettings() { FAAMaxPagesToTryPerMapping = 100 };
            fakeApprenticeshipVacancyApi = A.Fake<IApprenticeshipVacancyApi>();
            fakeMapper = A.Fake<AutoMapper.IMapper>();

            aVMapping = new AVMapping
            {
                Standards = new string[] { "225" },
                Frameworks = new string[] { "512" },
            };
        }

        [Fact]
        public async Task GetAVSumaryPageTestAsync()
        {
            //arrange
            var pageNumber = 1;
            var pageSize = 5;
            var returnDiffrentProvidersOnPage = 1;
            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Search)).Returns(AVAPIDummyResponses.GetDummyApprenticeshipVacancySummaryResponse(pageNumber, 50, pageSize, pageSize, returnDiffrentProvidersOnPage));
            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Act
            var pageSumary = await aVAPIService.GetAVSumaryPageAsync(aVMapping, 1).ConfigureAwait(false);

            //Asserts
            pageSumary.CurrentPage.Should().Be(pageNumber);
            pageSumary.Results.Count().Should().Be(pageSize);

            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Search)).MustHaveHappened();
        }

        [Fact]
        public void GetAVSumaryPageAsyncNullExceptionsTest()
        {
            //Setup
            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Asserts
            Func<Task> f = async () => { await aVAPIService.GetAVSumaryPageAsync(null, 1).ConfigureAwait(false); };
            f.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAVsForMultipleProvidersTestAsync()
        {
            //Arrange
            var pageNumber = 1;
            var pageSize = 5;
            var returnDiffrentProvidersOnPage = 2;

            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Search)).Returns(AVAPIDummyResponses.GetDummyApprenticeshipVacancySummaryResponse(pageNumber, 50, pageSize, pageSize, returnDiffrentProvidersOnPage)).Once().
                Then.Returns(AVAPIDummyResponses.GetDummyApprenticeshipVacancySummaryResponse(pageNumber + 1, 50, pageSize, pageSize, returnDiffrentProvidersOnPage));

            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Act
            var aVSumaryList = await aVAPIService.GetAVsForMultipleProvidersAsync(aVMapping).ConfigureAwait(false);

            //Asserts
            //must have got more then 1 page to get multipe supplier
            aVSumaryList.Count().Should().BeGreaterThan(pageSize);

            var numberProviders = aVSumaryList.Select(v => v.TrainingProviderName).Distinct().Count();
            numberProviders.Should().BeGreaterThan(1);

            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Search)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void GetAVsForMultipleProvidersAsyncNullExceptionsTest()
        {
            //Setup
            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Asserts
            Func<Task> f = async () => { await aVAPIService.GetAVsForMultipleProvidersAsync(null).ConfigureAwait(false); };
            f.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetApprenticeshipVacancyDetailsTestAsync()
        {
            //Arrange
            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Apprenticeships)).Returns(AVAPIDummyResponses.GetDummyApprenticeshipVacancyDetailsResponse());

            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Act
            var vacancyDetails = await aVAPIService.GetApprenticeshipVacancyDetailsAsync(123).ConfigureAwait(false);

            //Asserts
            vacancyDetails.Should().NotBeNull();
            vacancyDetails.VacancyReference.Should().Be(123);

            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.Apprenticeships)).MustHaveHappened();
        }
    }
}
