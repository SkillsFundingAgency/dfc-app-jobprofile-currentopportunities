using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "GetAll Tests")]
    public class SegmentServiceGetAllTests
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly ICourseCurrentOpportuntiesRefresh fakeCourseCurrentOpportuntiesRefresh;
        private readonly IAVCurrentOpportuntiesRefresh fakeAVCurrentOpportunatiesRefresh;
        private readonly ILogger<CurrentOpportunitiesSegmentService> fakeLogger;
        private readonly IMapper fakeMapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> fakeJobProfileSegmentRefreshService;

        public SegmentServiceGetAllTests()
        {
            repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            fakeCourseCurrentOpportuntiesRefresh = A.Fake<ICourseCurrentOpportuntiesRefresh>();
            fakeAVCurrentOpportunatiesRefresh = A.Fake<IAVCurrentOpportuntiesRefresh>();
            fakeLogger = A.Fake<ILogger<CurrentOpportunitiesSegmentService>>();
            fakeMapper = A.Fake<IMapper>();
            fakeJobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();

            currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, fakeCourseCurrentOpportuntiesRefresh, fakeAVCurrentOpportunatiesRefresh, fakeLogger, fakeMapper, fakeJobProfileSegmentRefreshService);
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
