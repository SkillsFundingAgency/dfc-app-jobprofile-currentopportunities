using DFC.App.JobProfile.CurrentOpportunities.Controllers;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Net.Http;
using System.Net.Mime;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.CourseFeedControllerTests
{
    public class CourseFeedControllerRefreshApprenticeshipsTests
    {
        public CourseFeedControllerRefreshApprenticeshipsTests()
        {
            FakeLogger = A.Fake<ILogger<FeedsController>>();
            FakeCourseCurrentOpportuntiesRefresh = A.Fake<ICourseCurrentOpportuntiesRefresh>();
        }

        protected ILogger<FeedsController> FakeLogger { get; }

        protected ICourseCurrentOpportuntiesRefresh FakeCourseCurrentOpportuntiesRefresh { get; }

        [Fact]
        public async void CourseFeedControllerRefreshApprenticeshipsReturnsSuccess()
        {
            // Arrange
            const int expectedResult = 9;
            var controller = BuildCourseFeedController();

            A.CallTo(() => FakeCourseCurrentOpportuntiesRefresh.RefreshCoursesAsync(A<Guid>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.RefreshCourses(Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCourseCurrentOpportuntiesRefresh.RefreshCoursesAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<FeedRefreshResponseViewModel>(jsonResult.Value);

            Assert.Equal(expectedResult, model.NumberPulled);
            Assert.Null(model.RequestErrorMessage);

            controller.Dispose();
        }

        [Fact]
        public async void CourseFeedControllerDRefreshApprenticeshipsExceptionReturnsError()
        {
            // Arrange
            const int expectedResult = 0;
            const string expectedErrorMessage = "System.Net.Http.HttpRequestException: Exception of type 'System.Net.Http.HttpRequestException' was thrown.";
            var controller = BuildCourseFeedController();

            A.CallTo(() => FakeCourseCurrentOpportuntiesRefresh.RefreshCoursesAsync(A<Guid>.Ignored)).Throws(new HttpRequestException());

            // Act
            var result = await controller.RefreshCourses(Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCourseCurrentOpportuntiesRefresh.RefreshCoursesAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<BadRequestObjectResult>(result);
            var model = Assert.IsAssignableFrom<FeedRefreshResponseViewModel>(jsonResult.Value);

            Assert.Equal(expectedResult, model.NumberPulled);
            Assert.StartsWith(expectedErrorMessage, model.RequestErrorMessage, StringComparison.OrdinalIgnoreCase);

            controller.Dispose();
        }

        private CourseFeedController BuildCourseFeedController()
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = MediaTypeNames.Application.Json;

            var controller = new CourseFeedController(FakeLogger, FakeCourseCurrentOpportuntiesRefresh)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}
