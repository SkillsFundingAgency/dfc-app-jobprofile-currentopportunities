using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "GetByName Tests")]
    public class SegmentServiceGetByNameTests : SegmentServiceBaseTests
    {
        public SegmentServiceGetByNameTests() : base()
        {
        }

        [Fact]
        public async Task SegmentServiceGetByNameReturnsSuccess()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = A.Fake<CurrentOpportunitiesSegmentModel>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await CurrentOpportunitiesSegmentService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task SegmentServiceGetByNameReturnsArgumentNullExceptionWhenNullIsUsed()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await CurrentOpportunitiesSegmentService.GetByNameAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

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
            var result = await CurrentOpportunitiesSegmentService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}