using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests
{
    public class JobProfileSegmentRefreshServiceTests
    {
        private readonly ITopicClient fakeTopicClient;
        private readonly ICorrelationIdProvider correlationIdProvider;
        private readonly ILogService fakeLogger;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> refreshService;

        public JobProfileSegmentRefreshServiceTests()
        {
            this.fakeTopicClient = A.Fake<ITopicClient>();
            this.correlationIdProvider = A.Fake<ICorrelationIdProvider>();
            this.fakeLogger = A.Fake<ILogService>();
            this.refreshService = new JobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>(fakeTopicClient, correlationIdProvider, fakeLogger);
        }

        [Fact]
        public async Task SendMessageSendsMessageOnTopicClient()
        {
            // Arrange
            var model = new RefreshJobProfileSegmentServiceBusModel
            {
                CanonicalName = "some-canonical-name-1",
                JobProfileId = Guid.NewGuid(),
                Segment = "CurrentOpportunities",
            };

            // Act
            await refreshService.SendMessageAsync(model).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeTopicClient.SendAsync(A<Message>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SendMessageListDoesNotSendMessagesWhenListIsNull()
        {
            // Act
            await refreshService.SendMessageListAsync(null).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeTopicClient.SendAsync(A<Message>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SendMessageLogsExceptions()
        {
            // Arrange
            var model = new RefreshJobProfileSegmentServiceBusModel
            {
                CanonicalName = "some-canonical-name-1",
                JobProfileId = Guid.NewGuid(),
                Segment = "CurrentOpportunities",
            };

            var topicClient = A.Fake<ITopicClient>();
            A.CallTo(() => topicClient.SendAsync(A<Message>.Ignored)).Throws(new ServiceBusException(false));

            var segmentRefreshService = new JobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>(topicClient, correlationIdProvider, fakeLogger);

            // Act
            await segmentRefreshService.SendMessageAsync(model).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeLogger.LogWarning(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SendMessageListSendsListOfMessagesOnTopicClient()
        {
            // Arrange
            var model = new List<RefreshJobProfileSegmentServiceBusModel>
            {
                new RefreshJobProfileSegmentServiceBusModel
                {
                    CanonicalName = "some-canonical-name-1",
                    JobProfileId = Guid.NewGuid(),
                    Segment = "CurrentOpportunities",
                },
                new RefreshJobProfileSegmentServiceBusModel
                {
                    CanonicalName = "some-canonical-name-2",
                    JobProfileId = Guid.NewGuid(),
                    Segment = "CurrentOpportunities",
                },
                new RefreshJobProfileSegmentServiceBusModel
                {
                    CanonicalName = "some-canonical-name-3",
                    JobProfileId = Guid.NewGuid(),
                    Segment = "CurrentOpportunities",
                },
            };

            // Act
            await refreshService.SendMessageListAsync(model).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeTopicClient.SendAsync(A<Message>.Ignored)).MustHaveHappened(model.Count, Times.Exactly);
        }
    }
}