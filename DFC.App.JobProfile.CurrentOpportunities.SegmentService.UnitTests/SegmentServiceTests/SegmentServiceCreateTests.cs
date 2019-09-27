using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.DraftSegmentService;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Create Tests")]
    public class SegmentServiceCreateTests
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly IDraftCurrentOpportunitiesSegmentService draftCurrentOpportunitiesSegmentService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;

        public SegmentServiceCreateTests()
        {
            repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            draftCurrentOpportunitiesSegmentService = A.Fake<DraftCurrentOpportunitiesSegmentService>();
            currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, draftCurrentOpportunitiesSegmentService);
        }

        [Fact]
        public void CareerPathSegmentServiceCreateReturnsSuccessWhenSegmentCreated()
        {
            // arrange
            var careerPathSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = A.Fake<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => repository.CreateAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.Created);
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = currentOpportunitiesSegmentService.CreateAsync(careerPathSegmentModel).Result;

            // assert
            A.CallTo(() => repository.CreateAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CareerPathSegmentServiceCreateReturnsArgumentNullExceptionWhenNullIsUsedAsync()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await currentOpportunitiesSegmentService.CreateAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: currentOpportunitiesSegmentModel", exceptionResult.Message);
        }

        [Fact]
        public void CareerPathSegmentServiceCreateReturnsNullWhenSegmentNotCreated()
        {
            // arrange
            var createOrUdateCareerPathSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = A.Dummy<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => repository.CreateAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = currentOpportunitiesSegmentService.CreateAsync(createOrUdateCareerPathSegmentModel).Result;

            // assert
            A.CallTo(() => repository.CreateAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void CareerPathSegmentServiceCreateReturnsNullWhenMissingRepository()
        {
            // arrange
            var careerPathSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            CurrentOpportunitiesSegmentModel expectedResult = null;

            A.CallTo(() => repository.CreateAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = currentOpportunitiesSegmentService.CreateAsync(careerPathSegmentModel).Result;

            // assert
            A.CallTo(() => repository.CreateAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }
    }
}
