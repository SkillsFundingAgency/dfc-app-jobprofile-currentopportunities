using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.SegmentControllerTests
{
    [Trait("Segment Controller", "Index Tests")]
    public class SegmentControllerIndexTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void SegmentControllerIndexHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const int resultsCount = 2;
            var expectedResults = A.CollectionOfFake<CurrentOpportunitiesSegmentModel>(resultsCount);
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetAllAsync()).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(A.Fake<IndexDocumentViewModel>());

            // Act
            var result = await controller.Index().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappened(resultsCount, Times.Exactly);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            Assert.Equal(resultsCount, model.Documents.Count());

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void SegmentControllerIndexHtmlReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const int resultsCount = 0;
            IEnumerable<CurrentOpportunitiesSegmentModel> expectedResults = null;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetAllAsync()).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(A.Fake<IndexDocumentViewModel>());

            // Act
            var result = await controller.Index().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappened(resultsCount, Times.Exactly);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            Assert.Equal(null, model.Documents);

            controller.Dispose();
        }
    }
}
