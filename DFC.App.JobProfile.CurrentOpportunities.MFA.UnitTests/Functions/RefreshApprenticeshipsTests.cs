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
    [Trait("Message Function App", "Refresh Apprenticeships Tests")]
    public class RefreshApprenticeshipsTests
    {
        private const int AbortAfterErrorCount = 5;
        private readonly TimerInfo timer;
        private readonly ILogger fakeLogger;
        private readonly IRefreshService fakeRefreshService;

        public RefreshApprenticeshipsTests()
        {
            timer = new TimerInfo(new ScheduleStub(), new ScheduleStatus());
            fakeLogger = A.Fake<ILogger>();
            fakeRefreshService = A.Fake<IRefreshService>();

            Environment.SetEnvironmentVariable(nameof(AbortAfterErrorCount), $"{AbortAfterErrorCount}");
        }

        [Fact]
        public async Task RefreshApprenticeshipsWhenSimpleListReturnsDataOK()
        {
            // Arrange
            var expectedModels = A.CollectionOfFake<SimpleJobProfileModel>(2);
            const HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            A.CallTo(() => fakeRefreshService.GetSimpleListAsync()).Returns(expectedModels);
            A.CallTo(() => fakeRefreshService.RefreshApprenticeshipsAsync(A<Guid>.Ignored)).Returns(expectedStatusCode);

            // Act
            await RefreshApprenticeships.RunAsync(timer, fakeLogger, fakeRefreshService).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRefreshService.GetSimpleListAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRefreshService.RefreshApprenticeshipsAsync(A<Guid>.Ignored)).MustHaveHappened(expectedModels.Count, Times.Exactly);
        }

        [Fact]
        public async Task RefreshApprenticeshipsWhenSimpleListReturnsNoDataOK()
        {
            // Arrange
            var expectedModels = A.CollectionOfFake<SimpleJobProfileModel>(0);
            const HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            A.CallTo(() => fakeRefreshService.GetSimpleListAsync()).Returns(expectedModels);
            A.CallTo(() => fakeRefreshService.RefreshApprenticeshipsAsync(A<Guid>.Ignored)).Returns(expectedStatusCode);

            // Act
            await RefreshApprenticeships.RunAsync(timer, fakeLogger, fakeRefreshService).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRefreshService.GetSimpleListAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRefreshService.RefreshApprenticeshipsAsync(A<Guid>.Ignored)).MustHaveHappened(expectedModels.Count, Times.Exactly);
        }

        [Fact]
        public async Task RefreshApprenticeshipsWhenMaxErrorsOccur()
        {
            // Arrange
            var expectedModels = A.CollectionOfFake<SimpleJobProfileModel>(AbortAfterErrorCount * 2);
            const HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

            A.CallTo(() => fakeRefreshService.GetSimpleListAsync()).Returns(expectedModels);
            A.CallTo(() => fakeRefreshService.RefreshApprenticeshipsAsync(A<Guid>.Ignored)).Returns(expectedStatusCode);

            // Act
            await RefreshApprenticeships.RunAsync(timer, fakeLogger, fakeRefreshService).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRefreshService.GetSimpleListAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRefreshService.RefreshApprenticeshipsAsync(A<Guid>.Ignored)).MustHaveHappened(AbortAfterErrorCount, Times.Exactly);
        }
    }
}
