﻿using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
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
        public async Task CurrentOpportunitiesSegementServiceCreateReturnsCreatedWhenSegmentCreated()
        {
            // arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = HttpStatusCode.Created;

            A.CallTo(() => FakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.Created);

            // act
            var result = await CurrentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => FakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CurrentOpportunitiesSegmentServiceCreateReturnsArgumentNullExceptionWhenNullIsUsedAsync()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await CurrentOpportunitiesSegmentService.UpsertAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null. (Parameter 'currentOpportunitiesSegmentModel')", exceptionResult.Message);
        }

        [Fact]
        public async Task CurrentOpportunitiesSegmentServiceCreateReturnsNullWhenSegmentNotCreated()
        {
            // arrange
            var createOrUdateCareerPathSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = A.Dummy<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => FakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = await CurrentOpportunitiesSegmentService.UpsertAsync(createOrUdateCareerPathSegmentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => FakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CurrentOpportunitiesSegmentServiceCreateReturnsNullWhenMissingRepository()
        {
            // arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = A.Dummy<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => FakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = await CurrentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => FakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Theory]
        [InlineData(HttpStatusCode.Created, true)]
        [InlineData(HttpStatusCode.OK, true)]
        [InlineData(HttpStatusCode.FailedDependency, false)]
        public async Task CurrentOpportunitiesSegmentServiceUpdateCoursesAndAppreticeshipsWhenUpserted(HttpStatusCode upsertReturnCode, bool shouldRefresh)
        {
            // arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            A.CallTo(() => FakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(upsertReturnCode);

            // act
            var result = await CurrentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => FakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            if (shouldRefresh)
            {
                A.CallTo(() => FakeCourseCurrentOpportunitiesRefresh.RefreshCoursesAsync(A<Guid>.Ignored)).MustHaveHappened();
                A.CallTo(() => FakeAVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(A<Guid>.Ignored)).MustHaveHappened();
            }
            else
            {
                A.CallTo(() => FakeCourseCurrentOpportunitiesRefresh.RefreshCoursesAsync(A<Guid>.Ignored)).MustNotHaveHappened();
                A.CallTo(() => FakeAVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            }
        }

        [Fact]
        public async Task RefreshJobProfileMessageSentWhenOnlyCourseServiceFails()
        {
            // arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            A.CallTo(() => FakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => FakeAVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(currentOpportunitiesSegmentModel.DocumentId)).Returns(2);
            A.CallTo(() => FakeCourseCurrentOpportunitiesRefresh.RefreshCoursesAsync(currentOpportunitiesSegmentModel.DocumentId)).Throws(new HttpRequestException());

            // act
            var result = await CurrentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // asserts
            A.CallTo(() => FakeJobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task RefreshJobProfileMessageSentWhenOnlyAVServiceFails()
        {
            // arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            A.CallTo(() => FakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => FakeAVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(currentOpportunitiesSegmentModel.DocumentId)).Throws(new HttpRequestException());
            A.CallTo(() => FakeCourseCurrentOpportunitiesRefresh.RefreshCoursesAsync(currentOpportunitiesSegmentModel.DocumentId)).Returns(2);

            // act
            var result = await CurrentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // asserts
            A.CallTo(() => FakeJobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task RefreshJobProfileMessageNotSentWhenCourseAndAVServiceFails()
        {
            // arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            A.CallTo(() => FakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => FakeAVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(currentOpportunitiesSegmentModel.DocumentId)).Throws(new HttpRequestException());
            A.CallTo(() => FakeCourseCurrentOpportunitiesRefresh.RefreshCoursesAsync(currentOpportunitiesSegmentModel.DocumentId)).Throws(new HttpRequestException());

            // act
            var result = await CurrentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // asserts
            A.CallTo(() => FakeJobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustNotHaveHappened();
        }
    }
}
