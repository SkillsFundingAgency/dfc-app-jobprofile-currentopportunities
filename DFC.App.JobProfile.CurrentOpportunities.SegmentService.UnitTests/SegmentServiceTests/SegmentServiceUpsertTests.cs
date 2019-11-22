using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Create Tests")]
    public class SegmentServiceUpsertTests : SegmentServiceBaseTests
    {
        public SegmentServiceUpsertTests() : base()
        {
        }

        [Fact]
        public void CurrentOpportunitiesSegementServiceCreateReturnsCreatedWhenSegmentCreated()
        {
            // arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = HttpStatusCode.Created;

            A.CallTo(() => repository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.Created);

            // act
            var result = currentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CurrentOpportunitiesSegmentServiceCreateReturnsArgumentNullExceptionWhenNullIsUsedAsync()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await currentOpportunitiesSegmentService.UpsertAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: currentOpportunitiesSegmentModel", exceptionResult.Message);
        }

        [Fact]
        public void CurrentOpportunitiesSegmentServiceCreateReturnsNullWhenSegmentNotCreated()
        {
            // arrange
            var createOrUdateCareerPathSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = A.Dummy<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => repository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = currentOpportunitiesSegmentService.UpsertAsync(createOrUdateCareerPathSegmentModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void CurrentOpportunitiesSegmentServiceCreateReturnsNullWhenMissingRepository()
        {
            // arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = A.Dummy<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => repository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = currentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Theory]
        [InlineData(HttpStatusCode.Created, true)]
        [InlineData(HttpStatusCode.OK, true)]
        [InlineData(HttpStatusCode.FailedDependency, false)]
        public void CurrentOpportunitiesSegmentServiceUpdateCoursesAndAppreticeshipsWhenUpserted(HttpStatusCode upsertReturnCode, bool shouldRefresh)
        {
            // arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            A.CallTo(() => repository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(upsertReturnCode);

            // act
            var result = currentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            if (shouldRefresh)
            {
                A.CallTo(() => fakeCourseCurrentOpportuntiesRefresh.RefreshCoursesAsync(A<Guid>.Ignored)).MustHaveHappened();
                A.CallTo(() => fakeAVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(A<Guid>.Ignored)).MustHaveHappened();
            }
            else
            {
                A.CallTo(() => fakeCourseCurrentOpportuntiesRefresh.RefreshCoursesAsync(A<Guid>.Ignored)).MustNotHaveHappened();
                A.CallTo(() => fakeAVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            }

        }
    }
}
