using DFC.App.JobProfile.CurrentOpportunities.Controllers;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.CourseFeedControllerTests
{
    public class CourseFeedControllerRefreshApprenticeshipsTests
    {
        public CourseFeedControllerRefreshApprenticeshipsTests()
        {
            FakeLogger = A.Fake<ILogService>();
            FakeCourseCurrentOpportunitiesRefresh = A.Fake<ICourseCurrentOpportunitiesRefresh>();
        }

        protected ILogService FakeLogger { get; }

        protected ICourseCurrentOpportunitiesRefresh FakeCourseCurrentOpportunitiesRefresh { get; }

        [Fact]
        public async Task CourseFeedControllerRefreshApprenticeshipsReturnsSuccess()
        {
            // Arrange
            const int expectedResult = 9;
            var controller = BuildCourseFeedController();

            A.CallTo(() => FakeCourseCurrentOpportunitiesRefresh.RefreshCoursesAndUpdateJobProfileAsync(A<Guid>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.RefreshCourses(Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCourseCurrentOpportunitiesRefresh.RefreshCoursesAndUpdateJobProfileAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<FeedRefreshResponseViewModel>(jsonResult.Value);

            Assert.Equal(expectedResult, model.NumberPulled);
            Assert.Null(model.RequestErrorMessage);

            controller.Dispose();
        }

        [Fact]
        public async Task CourseFeedControllerDRefreshApprenticeshipsExceptionReturnsError()
        {
            // Arrange
            const int expectedResult = 0;
            const string expectedErrorMessage = "System.Net.Http.HttpRequestException: Exception of type 'System.Net.Http.HttpRequestException' was thrown.";
            var controller = BuildCourseFeedController();

            A.CallTo(() => FakeCourseCurrentOpportunitiesRefresh.RefreshCoursesAndUpdateJobProfileAsync(A<Guid>.Ignored)).Throws(new HttpRequestException());

            // Act
            var result = await controller.RefreshCourses(Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCourseCurrentOpportunitiesRefresh.RefreshCoursesAndUpdateJobProfileAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

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

            var controller = new CourseFeedController(FakeLogger, FakeCourseCurrentOpportunitiesRefresh)
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