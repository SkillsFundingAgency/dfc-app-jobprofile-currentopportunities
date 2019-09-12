using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.SegmentControllerTests
{
    [Trait("Segment Controller", "Body Tests")]
    public class SegmentControllerBodyTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void SegmentControllerBodyHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<CurrentOpportunitiesSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            var dummyBodyViewModel = A.Dummy<BodyViewModel>();
            dummyBodyViewModel.BodyData = A.Dummy<BodyDataViewModel>();

            A.CallTo(() => FakeCareerPathSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(dummyBodyViewModel);

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCareerPathSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void SegmentControllerBodyHtmlReturnsNoContentWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            CurrentOpportunitiesSegmentModel expectedResult = null;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCareerPathSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCareerPathSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}