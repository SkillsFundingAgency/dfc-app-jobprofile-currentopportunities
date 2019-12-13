using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "GetAll Tests")]
    public class SegmentServiceGetAllTests : SegmentServiceBaseTests
    {

        public SegmentServiceGetAllTests() : base()
        {
        }

        [Fact]
        public async Task SegmentServiceGetAllListReturnsSuccess()
        {
            // arrange
            var expectedResults = A.CollectionOfFake<CurrentOpportunitiesSegmentModel>(2);

            A.CallTo(() => repository.GetAllAsync()).Returns(expectedResults);

            // act
            var results = await CurrentOpportunitiesSegmentService.GetAllAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.Equals(results, expectedResults);
        }

        [Fact]
        public async Task SegmentServiceGetAllListReturnsNullWhenMissingRepository()
        {
            // arrange
            IEnumerable<CurrentOpportunitiesSegmentModel> expectedResults = null;

            A.CallTo(() => repository.GetAllAsync()).Returns(expectedResults);

            // act
            var results = await CurrentOpportunitiesSegmentService.GetAllAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.Equals(results, expectedResults);
        }
    }
}
