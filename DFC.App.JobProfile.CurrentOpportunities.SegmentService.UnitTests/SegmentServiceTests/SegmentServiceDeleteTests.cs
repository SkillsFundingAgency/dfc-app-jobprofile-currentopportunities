using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Delete Tests")]
    public class SegmentServiceDeleteTests : SegmentServiceBaseTests
    {
        public SegmentServiceDeleteTests() : base()
        {
        }

        [Fact]
        public void CareerPathSegmentServiceDeleteReturnsSuccessWhenSegmentDeleted()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = true;

            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).Returns(HttpStatusCode.NoContent);

            // act
            var result = CurrentOpportunitiesSegmentService.DeleteAsync(documentId).Result;

            // assert
            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void CareerPathSegmentServiceDeleteReturnsNullWhenSegmentNotDeleted()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = false;

            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = CurrentOpportunitiesSegmentService.DeleteAsync(documentId).Result;

            // assert
            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void CareerPathSegmentServiceDeleteReturnsFalseWhenMissingRepository()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var careerPathSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = false;

            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = CurrentOpportunitiesSegmentService.DeleteAsync(documentId).Result;

            // assert
            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}
