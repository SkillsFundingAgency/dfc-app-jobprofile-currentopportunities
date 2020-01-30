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
        public static IEnumerable<object[]> BatchSizeData => new List<object[]>
        {
            new object[] { 1, 1 },
            new object[] { 499, 1 },
            new object[] { 500, 1 },
            new object[] { 501, 2 },
            new object[] { 999, 2 },
            new object[] { 1000, 2 },
            new object[] { 1001, 3 },
            new object[] { 1499, 3 },
            new object[] { 1500, 3 },
            new object[] { 1501, 4 },
        };

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

            var models = CreateListOfModels();

            // Act
            await refreshService.SendMessageListAsync(models).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeTopicClient.SendAsync(A<List<Message>>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [MemberData(nameof(BatchSizeData))]
        public async Task SendMessageListSendsBatchedMessagesOnTopicClient(int batchSize, int expectedSentBatches)
        {
            // Arrange
            var fakeTopicClient = A.Fake<ITopicClient>();
            var correlationIdProvider = A.Fake<ICorrelationIdProvider>();
            var refreshService = new JobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>(fakeTopicClient, correlationIdProvider);
            var models = CreateListOfModels(batchSize);

            // Act
            await refreshService.SendMessageListAsync(models).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeTopicClient.SendAsync(A<List<Message>>.Ignored)).MustHaveHappened(expectedSentBatches, Times.Exactly);
        }

        private List<RefreshJobProfileSegmentServiceBusModel> CreateListOfModels(int numModels = 3)
        {
            var result = new List<RefreshJobProfileSegmentServiceBusModel>();
            for (var i = 0; i < numModels; i++)
            {
                result.Add(new RefreshJobProfileSegmentServiceBusModel
                {
                    CanonicalName = $"some-canonical-name-{i}",
                    JobProfileId = Guid.NewGuid(),
                    Segment = "CurrentOpportunities",
                });
            }

            return result;
        }
    }
}