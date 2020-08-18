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

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.FeedsControllerTests
{
    public class FeedsControllerRefreshApprenticeshipsTests
    {
        public FeedsControllerRefreshApprenticeshipsTests()
        {
            FakeLogger = A.Fake<ILogService>();
            FakeIAVCurrentOpportunitiesRefresh = A.Fake<IAVCurrentOpportunitiesRefresh>();
        }

        protected ILogService FakeLogger { get; }

        protected IAVCurrentOpportunitiesRefresh FakeIAVCurrentOpportunitiesRefresh { get; }

        [Fact]
        public async Task FeedsControllerRefreshApprenticeshipsReturnsSuccess()
        {
            // Arrange
            const int expectedResult = 9;
            var controller = BuildFeedsController();

            A.CallTo(() => FakeIAVCurrentOpportunitiesRefresh.RefreshApprenticeshipVacanciesAndUpdateJobProfileAsync(A<Guid>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.RefreshApprenticeships(Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeIAVCurrentOpportunitiesRefresh.RefreshApprenticeshipVacanciesAndUpdateJobProfileAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<FeedRefreshResponseViewModel>(jsonResult.Value);

            Assert.Equal(expectedResult, model.NumberPulled);
            Assert.Null(model.RequestErrorMessage);

            controller.Dispose();
        }

        [Fact]
        public async Task FeedsControllerDRefreshApprenticeshipsExceptionReturnsError()
        {
            // Arrange
            const int expectedResult = 0;
            const string expectedErrorMessage = "Exception of type 'System.Net.Http.HttpRequestException' was thrown.";
            var controller = BuildFeedsController();

            A.CallTo(() => FakeIAVCurrentOpportunitiesRefresh.RefreshApprenticeshipVacanciesAndUpdateJobProfileAsync(A<Guid>.Ignored)).Throws(new HttpRequestException());

            // Act
            var result = await controller.RefreshApprenticeships(Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeIAVCurrentOpportunitiesRefresh.RefreshApprenticeshipVacanciesAndUpdateJobProfileAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<BadRequestObjectResult>(result);
            var model = Assert.IsAssignableFrom<FeedRefreshResponseViewModel>(jsonResult.Value);

            Assert.Equal(expectedResult, model.NumberPulled);
            Assert.Equal(expectedErrorMessage, model.RequestErrorMessage);

            controller.Dispose();
        }

        private FeedsController BuildFeedsController()
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = MediaTypeNames.Application.Json;

            var controller = new FeedsController(FakeLogger, FakeIAVCurrentOpportunitiesRefresh)
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