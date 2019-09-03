using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.DraftSegmentService;
using FakeItEasy;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "GetAll Tests")]
    public class SegmentServiceGetAllTests
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly IDraftCurrentOpportunitiesSegmentService draftCurrentOpportunitiesSegmentService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;

        public SegmentServiceGetAllTests()
        {
            repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            draftCurrentOpportunitiesSegmentService = A.Fake<DraftCurrentOpportunitiesSegmentService>();
            currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, draftCurrentOpportunitiesSegmentService);
        }

        [Fact]
        public async Task SegmentServiceGetAllListReturnsSuccess()
        {
            // arrange
            var expectedResults = A.CollectionOfFake<CurrentOpportunitiesSegmentModel>(2);

            A.CallTo(() => repository.GetAllAsync()).Returns(expectedResults);

            // act
            var results = await currentOpportunitiesSegmentService.GetAllAsync().ConfigureAwait(false);

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
            var results = await currentOpportunitiesSegmentService.GetAllAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.Equals(results, expectedResults);
        }
    }
}
