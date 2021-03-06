using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "GetById Tests")]
    public class SegmentServiceGetByIdTests : SegmentServiceBaseTests
    {
        public SegmentServiceGetByIdTests() : base()
        {
        }

        [Fact]
        public async Task SegmentServiceGetByIdReturnsSuccess()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = A.Fake<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => FakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await CurrentOpportunitiesSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => FakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task SegmentServiceGetByIdReturnsNullWhenMissingInRepository()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            CurrentOpportunitiesSegmentModel expectedResult = null;

            A.CallTo(() => FakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await CurrentOpportunitiesSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => FakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}