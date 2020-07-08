using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using System;
using System.Net;
using System.Threading.Tasks;
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
        public async Task CareerPathSegmentServiceDeleteReturnsSuccessWhenSegmentDeleted()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = true;

            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).Returns(HttpStatusCode.NoContent);

            // act
            var result = await CurrentOpportunitiesSegmentService.DeleteAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CareerPathSegmentServiceDeleteReturnsNullWhenSegmentNotDeleted()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = false;

            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = await CurrentOpportunitiesSegmentService.DeleteAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CareerPathSegmentServiceDeleteReturnsFalseWhenMissingRepository()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var careerPathSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = false;

            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = await CurrentOpportunitiesSegmentService.DeleteAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => FakeRepository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}
