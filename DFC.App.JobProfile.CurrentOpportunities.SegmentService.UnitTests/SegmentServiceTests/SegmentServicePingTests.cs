using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using DFC.App.JobProfile.CurrentOpportunities.SegmentService;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.CareerPath.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Ping / Health Tests")]
    public class SegmentServicePingTests
    {
        private readonly IDraftCurrentOpportunitiesSegmentService draftCurrentOpportunitiesSegmentService;
        private readonly ICourseCurrentOpportuntiesRefresh fakeCourseCurrentOpportuntiesRefresh;
        private readonly IAVCurrentOpportuntiesRefresh fakeAVCurrentOpportunatiesRefresh;
        private readonly ILogger<CurrentOpportunitiesSegmentService> fakeLogger;
        private readonly IMapper fakeMapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> fakeJobProfileSegmentRefreshService;

        public SegmentServicePingTests()
        {
            draftCurrentOpportunitiesSegmentService = A.Fake<IDraftCurrentOpportunitiesSegmentService>();
            fakeCourseCurrentOpportuntiesRefresh = A.Fake<ICourseCurrentOpportuntiesRefresh>();
            fakeAVCurrentOpportunatiesRefresh = A.Fake<IAVCurrentOpportuntiesRefresh>();
            fakeLogger = A.Fake<ILogger<CurrentOpportunitiesSegmentService>>();
        }

        [Fact]
        public void CareerPathSegmentServicePingReturnsSuccess()
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            var expectedResult = true;
            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);
            var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, fakeCourseCurrentOpportuntiesRefresh, fakeAVCurrentOpportunatiesRefresh, fakeLogger, fakeMapper, fakeJobProfileSegmentRefreshService);

            // act
            var result = currentOpportunitiesSegmentService.PingAsync().Result;

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void CareerPathSegmentServicePingReturnsFalseWhenMissingRepository()
        {
            // arrange
            var repository = A.Dummy<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            var expectedResult = false;

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);
            var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, fakeCourseCurrentOpportuntiesRefresh, fakeAVCurrentOpportunatiesRefresh, fakeLogger, fakeMapper, fakeJobProfileSegmentRefreshService);

            // act
            var result = currentOpportunitiesSegmentService.PingAsync().Result;

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}