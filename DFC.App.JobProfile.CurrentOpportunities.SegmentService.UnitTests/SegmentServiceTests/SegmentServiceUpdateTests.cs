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
    [Trait("Profile Service", "Update Tests")]
    public class SegmentServiceUpdateTests
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly IDraftCurrentOpportunitiesSegmentService draftCurrentOpportunitiesSegmentService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;

        public SegmentServiceUpdateTests()
        {
            repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            draftCurrentOpportunitiesSegmentService = A.Fake<DraftCurrentOpportunitiesSegmentService>();
            currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, draftCurrentOpportunitiesSegmentService);
        }

        [Fact]
        public void CareerPathSegmentServiceUpdateReturnsSuccessWhenProfileReplaced()
        {
            // arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = A.Fake<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => repository.UpdateAsync(currentOpportunitiesSegmentModel.DocumentId, currentOpportunitiesSegmentModel)).Returns(HttpStatusCode.OK);
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = currentOpportunitiesSegmentService.ReplaceAsync(currentOpportunitiesSegmentModel).Result;

            // assert
            A.CallTo(() => repository.UpdateAsync(currentOpportunitiesSegmentModel.DocumentId, currentOpportunitiesSegmentModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CareerPathSegmentServiceUpdateReturnsArgumentNullExceptionWhenNullParamIsUsed()
        {
            // arrange
            CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel = null;

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await currentOpportunitiesSegmentService.ReplaceAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: careerPathSegmentModel", exceptionResult.Message);
        }

        [Fact]
        public void CareerPathSegmentServiceUpdateReturnsNullWhenProfileNotReplaced()
        {
            // arrange
            var careerPathSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = A.Dummy<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => repository.UpdateAsync(careerPathSegmentModel.DocumentId, careerPathSegmentModel)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = currentOpportunitiesSegmentService.ReplaceAsync(careerPathSegmentModel).Result;

            // assert
            A.CallTo(() => repository.UpdateAsync(careerPathSegmentModel.DocumentId, careerPathSegmentModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void CareerPathSegmentServiceUpdateReturnsNullWhenMissingRepository()
        {
            // arrange
            var careerPathSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            CurrentOpportunitiesSegmentModel expectedResult = null;

            A.CallTo(() => repository.UpdateAsync(careerPathSegmentModel.DocumentId, careerPathSegmentModel)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = currentOpportunitiesSegmentService.ReplaceAsync(careerPathSegmentModel).Result;

            // assert
            A.CallTo(() => repository.UpdateAsync(careerPathSegmentModel.DocumentId, careerPathSegmentModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }
    }
}
