using FakeItEasy;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Ping / Health Tests")]
    public class SegmentServicePingTests : SegmentServiceBaseTests
    {
        public SegmentServicePingTests() : base()
        {
        }

        [Fact]
        public void CareerPathSegmentServicePingReturnsSuccess()
        {
            // arrange
            var expectedResult = true;
            A.CallTo(() => FakeRepository.PingAsync()).Returns(expectedResult);

            // act
            var result = CurrentOpportunitiesSegmentService.PingAsync().Result;

            // assert
            A.CallTo(() => FakeRepository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void CareerPathSegmentServicePingReturnsFalseWhenMissingRepository()
        {
            // arrange
            var expectedResult = false;

            A.CallTo(() => FakeRepository.PingAsync()).Returns(expectedResult);

            // act
            var result = CurrentOpportunitiesSegmentService.PingAsync().Result;

            // assert
            A.CallTo(() => FakeRepository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}