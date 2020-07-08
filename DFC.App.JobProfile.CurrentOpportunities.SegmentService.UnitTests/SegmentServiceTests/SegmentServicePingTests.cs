using FakeItEasy;
using System.Threading.Tasks;
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
        public async Task CareerPathSegmentServicePingReturnsSuccess()
        {
            // arrange
            var expectedResult = true;
            A.CallTo(() => FakeRepository.PingAsync()).Returns(expectedResult);

            // act
            var result = await CurrentOpportunitiesSegmentService.PingAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => FakeRepository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CareerPathSegmentServicePingReturnsFalseWhenMissingRepository()
        {
            // arrange
            var expectedResult = false;

            A.CallTo(() => FakeRepository.PingAsync()).Returns(expectedResult);

            // act
            var result = await CurrentOpportunitiesSegmentService.PingAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => FakeRepository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}