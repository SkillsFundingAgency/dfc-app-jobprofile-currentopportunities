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
        [Fact]
        public async Task SendMessageSendsMessageOnTopicClient()
        {
            // Arrange
            var fakeTopicClient = A.Fake<ITopicClient>();
            var correlationIdProvider = A.Fake<ICorrelationIdProvider>();
            var refreshService = new JobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>(fakeTopicClient, correlationIdProvider);

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
            // Arrange
            var fakeTopicClient = A.Fake<ITopicClient>();
            var correlationIdProvider = A.Fake<ICorrelationIdProvider>();
            var refreshService = new JobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>(fakeTopicClient, correlationIdProvider);

            // Act
            await refreshService.SendMessageListAsync(null).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeTopicClient.SendAsync(A<Message>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SendMessageListSendsListOfMessagesOnTopicClient()
        {
            // Arrange
            var fakeTopicClient = A.Fake<ITopicClient>();
            var correlationIdProvider = A.Fake<ICorrelationIdProvider>();
            var refreshService = new JobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>(fakeTopicClient, correlationIdProvider);

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
            A.CallTo(() => fakeTopicClient.SendAsync(A<List<Message>>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}