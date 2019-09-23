using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.SegmentService;
using FakeItEasy;
using Xunit;

namespace DFC.App.CareerPath.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Ping / Health Tests")]
    public class SegmentServicePingTests
    {
        [Fact]
        public void CareerPathSegmentServicePingReturnsSuccess()
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            var expectedResult = true;

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, A.Fake<IDraftCurrentOpportunitiesSegmentService>());

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

            var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, A.Fake<IDraftCurrentOpportunitiesSegmentService>());

            // act
            var result = currentOpportunitiesSegmentService.PingAsync().Result;

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}