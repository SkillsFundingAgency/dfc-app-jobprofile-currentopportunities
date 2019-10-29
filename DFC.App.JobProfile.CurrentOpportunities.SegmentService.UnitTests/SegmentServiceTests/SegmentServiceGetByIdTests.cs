using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.DraftSegmentService;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "GetById Tests")]
    public class SegmentServiceGetByIdTests
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly IDraftCurrentOpportunitiesSegmentService draftCurrentOpportunitiesSegmentService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly ICourseCurrentOpportuntiesRefresh fakeCourseCurrentOpportuntiesRefresh;
        private readonly IAVCurrentOpportuntiesRefresh fakeAVCurrentOpportunatiesRefresh;
        private readonly ILogger<CurrentOpportunitiesSegmentService> fakeLogger;

        public SegmentServiceGetByIdTests()
        {
            repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            draftCurrentOpportunitiesSegmentService = A.Fake<DraftCurrentOpportunitiesSegmentService>();
            fakeCourseCurrentOpportuntiesRefresh = A.Fake<ICourseCurrentOpportuntiesRefresh>();
            fakeAVCurrentOpportunatiesRefresh = A.Fake<IAVCurrentOpportuntiesRefresh>();
            fakeLogger = A.Fake<ILogger<CurrentOpportunitiesSegmentService>>();

            currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, draftCurrentOpportunitiesSegmentService, fakeCourseCurrentOpportuntiesRefresh, fakeAVCurrentOpportunatiesRefresh, fakeLogger);
        }

        [Fact]
        public async Task SegmentServiceGetByIdReturnsSuccess()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = A.Fake<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await currentOpportunitiesSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task SegmentServiceGetByIdReturnsNullWhenMissingInRepository()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            CurrentOpportunitiesSegmentModel expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await currentOpportunitiesSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}