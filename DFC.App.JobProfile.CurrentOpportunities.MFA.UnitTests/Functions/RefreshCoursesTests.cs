using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Functions;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Models;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using FakeItEasy;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests.Functions
{
    [Trait("Message Function App", "Refresh Courses Tests")]
    public class RefreshCoursesTests
    {
        private const int AbortAfterErrorCount = 5;
        private readonly TimerInfo timer;
        private readonly ILogger fakeLogger;
        private readonly IRefreshService fakeRefreshService;

        public RefreshCoursesTests()
        {
            timer = new TimerInfo(new ScheduleStub(), new ScheduleStatus());
            fakeLogger = A.Fake<ILogger>();
            fakeRefreshService = A.Fake<IRefreshService>();

            Environment.SetEnvironmentVariable(nameof(AbortAfterErrorCount), $"{AbortAfterErrorCount}");
        }

        [Fact]
        public async Task RefreshCoursesWhenSimpleListReturnsDataOK()
        {
            // Arrange
            var expectedModels = A.CollectionOfFake<SimpleJobProfileModel>(2);
            const HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            A.CallTo(() => fakeRefreshService.GetListAsync()).Returns(expectedModels);
            A.CallTo(() => fakeRefreshService.RefreshCoursesAsync(A<Guid>.Ignored)).Returns(expectedStatusCode);

            // Act
            await RefreshCourses.RunAsync(timer, fakeLogger, fakeRefreshService).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRefreshService.GetListAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRefreshService.RefreshCoursesAsync(A<Guid>.Ignored)).MustHaveHappened(expectedModels.Count, Times.Exactly);
        }

        [Fact]
        public async Task RefreshCoursesWhenSimpleListReturnsNoDataOK()
        {
            // Arrange
            var expectedModels = A.CollectionOfFake<SimpleJobProfileModel>(0);
            const HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            A.CallTo(() => fakeRefreshService.GetListAsync()).Returns(expectedModels);
            A.CallTo(() => fakeRefreshService.RefreshCoursesAsync(A<Guid>.Ignored)).Returns(expectedStatusCode);

            // Act
            await RefreshCourses.RunAsync(timer, fakeLogger, fakeRefreshService).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRefreshService.GetListAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRefreshService.RefreshCoursesAsync(A<Guid>.Ignored)).MustHaveHappened(expectedModels.Count, Times.Exactly);
        }

        [Fact]
        public async Task RefreshCoursesWhenMaxErrorsOccur()
        {
            // Arrange
            var expectedModels = A.CollectionOfFake<SimpleJobProfileModel>(AbortAfterErrorCount * 2);
            const HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

            A.CallTo(() => fakeRefreshService.GetListAsync()).Returns(expectedModels);
            A.CallTo(() => fakeRefreshService.RefreshCoursesAsync(A<Guid>.Ignored)).Returns(expectedStatusCode);

            // Act
            await RefreshCourses.RunAsync(timer, fakeLogger, fakeRefreshService).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRefreshService.GetListAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRefreshService.RefreshCoursesAsync(A<Guid>.Ignored)).MustHaveHappened(AbortAfterErrorCount, Times.Exactly);
        }
    }
}
