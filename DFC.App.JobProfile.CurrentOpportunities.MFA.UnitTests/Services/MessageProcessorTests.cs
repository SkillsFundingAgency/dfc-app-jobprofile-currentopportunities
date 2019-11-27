using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels.PatchModels;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using FakeItEasy;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests.Services
{
    [Trait("Messaging Function", "Message Processor Tests")]
    public class MessageProcessorTests
    {
        private readonly IMapper mapper;
        private readonly IHttpClientService httpClientService;
        private readonly IMappingService mappingService;
        private readonly IMessageProcessor messageProcessor;

        public MessageProcessorTests()
        {
            mapper = A.Fake<IMapper>();
            httpClientService = A.Fake<IHttpClientService>();
            mappingService = A.Fake<IMappingService>();

            messageProcessor = new MessageProcessor(mapper, httpClientService, mappingService);
        }

        [Fact]
        public async Task ProcessAsyncJobProfileSocTestReturnsOk()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            const MessageContentType messageContentType = MessageContentType.JobProfileSoc;
            const string message = "{}";
            const long sequenceNumber = 1;

            A.CallTo(() => mapper.Map<PatchJobProfileSocModel>(A<PatchJobProfileSocServiceBusModel>.Ignored)).Returns(A.Fake<PatchJobProfileSocModel>());
            A.CallTo(() => httpClientService.PatchAsync(A<PatchJobProfileSocModel>.Ignored, A<string>.Ignored)).Returns(expectedResult);

            // act
            var result = await messageProcessor.ProcessAsync(message, sequenceNumber, messageContentType, MessageAction.Published).ConfigureAwait(false);

            // assert
            A.CallTo(() => mapper.Map<PatchJobProfileSocModel>(A<PatchJobProfileSocServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PatchAsync(A<PatchJobProfileSocModel>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessAsyncApprenticeshipFrameworksTestReturnsOk()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            const MessageContentType messageContentType = MessageContentType.ApprenticeshipFrameworks;
            const string message = "{}";
            const long sequenceNumber = 1;

            A.CallTo(() => mapper.Map<PatchApprenticeshipFrameworksModel>(A<PatchApprenticeshipFrameworksServiceBusModel>.Ignored)).Returns(A.Fake<PatchApprenticeshipFrameworksModel>());
            A.CallTo(() => httpClientService.PatchAsync(A<PatchApprenticeshipFrameworksModel>.Ignored, A<string>.Ignored)).Returns(expectedResult);

            // act
            var result = await messageProcessor.ProcessAsync(message, sequenceNumber, messageContentType, MessageAction.Published).ConfigureAwait(false);

            // assert
            A.CallTo(() => mapper.Map<PatchApprenticeshipFrameworksModel>(A<PatchApprenticeshipFrameworksServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PatchAsync(A<PatchApprenticeshipFrameworksModel>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessAsyncApprenticeshipStandardsTestReturnsOk()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            const MessageContentType messageContentType = MessageContentType.ApprenticeshipStandards;
            const string message = "{}";
            const long sequenceNumber = 1;

            A.CallTo(() => mapper.Map<PatchApprenticeshipStandardsModel>(A<PatchApprenticeshipStandardsServiceBusModel>.Ignored)).Returns(A.Fake<PatchApprenticeshipStandardsModel>());
            A.CallTo(() => httpClientService.PatchAsync(A<PatchApprenticeshipStandardsModel>.Ignored, A<string>.Ignored)).Returns(expectedResult);

            // act
            var result = await messageProcessor.ProcessAsync(message, sequenceNumber, messageContentType, MessageAction.Published).ConfigureAwait(false);

            // assert
            A.CallTo(() => mapper.Map<PatchApprenticeshipStandardsModel>(A<PatchApprenticeshipStandardsServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PatchAsync(A<PatchApprenticeshipStandardsModel>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessAsyncJobProfileCreatePublishedTestReturnsOk()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.Created;
            const string message = "{}";
            const long sequenceNumber = 1;

            A.CallTo(() => mappingService.MapToSegmentModel(message, sequenceNumber)).Returns(A.Fake<CurrentOpportunitiesSegmentModel>());
            A.CallTo(() => httpClientService.PutAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(expectedResult);

            // act
            var result = await messageProcessor.ProcessAsync(message, sequenceNumber, MessageContentType.JobProfile, MessageAction.Published).ConfigureAwait(false);

            // assert
            A.CallTo(() => mappingService.MapToSegmentModel(message, sequenceNumber)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PutAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessAsyncJobProfileUpdatePublishedTestReturnsOk()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            const string message = "{}";
            const long sequenceNumber = 1;

            A.CallTo(() => mappingService.MapToSegmentModel(message, sequenceNumber)).Returns(A.Fake<CurrentOpportunitiesSegmentModel>());
            A.CallTo(() => httpClientService.PutAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => httpClientService.PostAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(expectedResult);

            // act
            var result = await messageProcessor.ProcessAsync(message, sequenceNumber, MessageContentType.JobProfile, MessageAction.Published).ConfigureAwait(false);

            // assert
            A.CallTo(() => mappingService.MapToSegmentModel(message, sequenceNumber)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PutAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PostAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessAsyncJobProfileDeletedTestReturnsOk()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            const string message = "{}";
            const long sequenceNumber = 1;

            A.CallTo(() => mappingService.MapToSegmentModel(message, sequenceNumber)).Returns(A.Fake<CurrentOpportunitiesSegmentModel>());
            A.CallTo(() => httpClientService.DeleteAsync(A<Guid>.Ignored)).Returns(expectedResult);

            // act
            var result = await messageProcessor.ProcessAsync(message, sequenceNumber, MessageContentType.JobProfile, MessageAction.Deleted).ConfigureAwait(false);

            // assert
            A.CallTo(() => mappingService.MapToSegmentModel(message, sequenceNumber)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessAsyncWithBadMessageContentTypeReturnsException()
        {
            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await messageProcessor.ProcessAsync(string.Empty, 1, (MessageContentType)(-1), MessageAction.Published).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Unexpected sitefinity content type '-1'\r\nParameter name: messageContentType", exceptionResult.Message);
        }
    }
}
