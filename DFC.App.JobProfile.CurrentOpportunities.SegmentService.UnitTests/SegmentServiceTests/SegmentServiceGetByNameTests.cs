using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "GetByName Tests")]
    public class SegmentServiceGetByNameTests
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly ICourseCurrentOpportuntiesRefresh fakeCourseCurrentOpportuntiesRefresh;
        private readonly IAVCurrentOpportuntiesRefresh fakeAVCurrentOpportunatiesRefresh;
        private readonly ILogger<CurrentOpportunitiesSegmentService> fakeLogger;
        private readonly IMapper fakeMapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> fakeJobProfileSegmentRefreshService;

        public SegmentServiceGetByNameTests()
        {
            repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            fakeCourseCurrentOpportuntiesRefresh = A.Fake<ICourseCurrentOpportuntiesRefresh>();
            fakeAVCurrentOpportunatiesRefresh = A.Fake<IAVCurrentOpportuntiesRefresh>();
            fakeLogger = A.Fake<ILogger<CurrentOpportunitiesSegmentService>>();

            currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, fakeCourseCurrentOpportuntiesRefresh, fakeAVCurrentOpportunatiesRefresh, fakeLogger, fakeMapper, fakeJobProfileSegmentRefreshService);
        }

        [Fact]
        public async Task SegmentServiceGetByNameReturnsSuccess()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = A.Fake<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await currentOpportunitiesSegmentService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task SegmentServiceGetByNameReturnsArgumentNullExceptionWhenNullIsUsed()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await currentOpportunitiesSegmentService.GetByNameAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: canonicalName", exceptionResult.Message);
        }

        [Fact]
        public async Task SegmentServiceGetByNameReturnsNullWhenMissingInRepository()
        {
            // arrange
            CurrentOpportunitiesSegmentModel expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await currentOpportunitiesSegmentService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}