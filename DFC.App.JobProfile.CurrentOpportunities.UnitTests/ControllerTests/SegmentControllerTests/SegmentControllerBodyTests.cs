using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.SegmentControllerTests
{
    [Trait("Segment Controller", "Body Tests")]
    public class SegmentControllerBodyTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task SegmentControllerBodyHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<CurrentOpportunitiesSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            var dummyBodyViewModel = A.Dummy<BodyViewModel>();
            dummyBodyViewModel.Data = A.Dummy<BodyDataViewModel>();
            dummyBodyViewModel.Data.Courses = A.Dummy<BodyCoursesViewModel>();

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(dummyBodyViewModel);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task SegmentControllerBodyJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<CurrentOpportunitiesSegmentModel>();
            expectedResult.Data = A.Dummy<CurrentOpportunitiesSegmentDataModel>();
            var dummyBodyViewModel = A.Dummy<BodyViewModel>();
            dummyBodyViewModel.Data = A.Dummy<BodyDataViewModel>();
            dummyBodyViewModel.Data.Courses = A.Dummy<BodyCoursesViewModel>();

            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(dummyBodyViewModel);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<CurrentOpportunitiesSegmentDataModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task SegmentControllerBodyReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<CurrentOpportunitiesSegmentModel>();
            var dummyBodyViewModel = A.Dummy<BodyViewModel>();
            dummyBodyViewModel.Data = A.Dummy<BodyDataViewModel>();
            dummyBodyViewModel.Data.Courses = A.Dummy<BodyCoursesViewModel>();

            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(dummyBodyViewModel);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task SegmentControllerBodyHtmlReturnsNoContentWhenNoData(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            CurrentOpportunitiesSegmentModel expectedResult = null;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NoContentResult>(result);

            Assert.Equal((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}