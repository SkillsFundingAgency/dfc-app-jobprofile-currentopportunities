using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Patch Tests")]
    public class SegmentServicePatchTests : SegmentServiceBaseTests
    {
        private readonly CurrentOpportunitiesSegmentPatchStatus currentOpportunitiesSegmentPatchStatus;
        private readonly CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel;
        private readonly Guid testGuid;

        public SegmentServicePatchTests() : base()
        {
            testGuid = Guid.NewGuid();
            currentOpportunitiesSegmentPatchStatus = new CurrentOpportunitiesSegmentPatchStatus()
            {
                OkToPatch = true,
                ReturnStatusCode = HttpStatusCode.OK,
            };
            A.CallTo(() => fakeCurrentOpportunitiesSegmentUtilities.IsSegementOkToPatch(A<CurrentOpportunitiesSegmentModel>.Ignored, A<long>.Ignored)).Returns(currentOpportunitiesSegmentPatchStatus);

            currentOpportunitiesSegmentModel = new CurrentOpportunitiesSegmentModel()
            {
                Data = new CurrentOpportunitiesSegmentDataModel()
                {
                },
            };
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
        }

        [Fact]
        public void PatchJobProfileSocNullTest()
        {
            //Asserts
            Func<Task> f = async () => { await CurrentOpportunitiesSegmentService.PatchJobProfileSocAsync(null, testGuid).ConfigureAwait(false); };
            f.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task JobProfileSocPatchSocReturnsNotFoundForMissingSegment()
        {
            //Arrange
            var doNotPatchResult = new CurrentOpportunitiesSegmentPatchStatus() { OkToPatch = false, ReturnStatusCode = HttpStatusCode.NotFound };
            var patchJobProfileSocModel = new PatchJobProfileSocModel() { ActionType = MessageAction.Deleted };
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(null as CurrentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCurrentOpportunitiesSegmentUtilities.IsSegementOkToPatch(A<CurrentOpportunitiesSegmentModel>.Ignored, A<long>.Ignored)).Returns(doNotPatchResult);

            //Act
            var result = await CurrentOpportunitiesSegmentService.PatchJobProfileSocAsync(patchJobProfileSocModel, testGuid).ConfigureAwait(false);

            //Asserts
            result.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task JobProfileSocPatchDeleteApprentishipNullTest()
        {
            //Arrange
            var patchJobProfileSocModel = new PatchJobProfileSocModel() { ActionType = MessageAction.Deleted };
            A.CallTo(() => fakeCurrentOpportunitiesSegmentUtilities.GetReturnStatusForNullElementPatchRequest(A<MessageAction>.Ignored)).Returns(HttpStatusCode.AlreadyReported);

            //Act
            var result = await CurrentOpportunitiesSegmentService.PatchJobProfileSocAsync(patchJobProfileSocModel, testGuid).ConfigureAwait(false);

            //Asserts
            result.Should().Be(HttpStatusCode.AlreadyReported);
        }

        [Theory]
        [InlineData(MessageAction.Deleted)]
        [InlineData(MessageAction.Published)]
        public async Task JobProfileSocPatchApprentishipTest(MessageAction messageAction)
        {
            //Arrange
            var patchJobProfileSocModel = new PatchJobProfileSocModel() { ActionType = messageAction };
            currentOpportunitiesSegmentModel.Data.Apprenticeships = new Apprenticeships();

            A.CallTo(() => repository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            //Act
            var result = await CurrentOpportunitiesSegmentService.PatchJobProfileSocAsync(patchJobProfileSocModel, testGuid).ConfigureAwait(false);

            //Asserts
            A.CallTo(() => fakeCurrentOpportunitiesSegmentUtilities.GetReturnStatusForNullElementPatchRequest(A<MessageAction>.Ignored)).MustNotHaveHappened();
            result.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void PatchApprenticeshipFrameworksNullTest()
        {
            //Asserts
            Func<Task> f = async () => { await CurrentOpportunitiesSegmentService.PatchApprenticeshipFrameworksAsync(null, testGuid).ConfigureAwait(false); };
            f.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task ApprenticeshipFrameworksPatchFrameworksNullTest()
        {
            //Arrange
            var patchApprenticeshipFrameworksModel = new PatchApprenticeshipFrameworksModel() { ActionType = MessageAction.Deleted };
            A.CallTo(() => fakeCurrentOpportunitiesSegmentUtilities.GetReturnStatusForNullElementPatchRequest(A<MessageAction>.Ignored)).Returns(HttpStatusCode.AlreadyReported);

            //Act
            var result = await CurrentOpportunitiesSegmentService.PatchApprenticeshipFrameworksAsync(patchApprenticeshipFrameworksModel, testGuid).ConfigureAwait(false);

            //Asserts
            result.Should().Be(HttpStatusCode.AlreadyReported);
        }

        [Fact]
        public async Task ApprenticeshipFrameworksPatchFrameworksReturnsNotFoundForMissingSegment()
        {
            //Arrange
            var doNotPatchResult = new CurrentOpportunitiesSegmentPatchStatus() { OkToPatch = false, ReturnStatusCode = HttpStatusCode.NotFound };
            var patchApprenticeshipFrameworksModel = new PatchApprenticeshipFrameworksModel() { ActionType = MessageAction.Deleted };
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(null as CurrentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCurrentOpportunitiesSegmentUtilities.IsSegementOkToPatch(A<CurrentOpportunitiesSegmentModel>.Ignored, A<long>.Ignored)).Returns(doNotPatchResult);

            //Act
            var result = await CurrentOpportunitiesSegmentService.PatchApprenticeshipFrameworksAsync(patchApprenticeshipFrameworksModel, testGuid).ConfigureAwait(false);

            //Asserts
            result.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(MessageAction.Deleted)]
        [InlineData(MessageAction.Published)]
        public async Task ApprenticeshipFrameworksPatchTest(MessageAction messageAction)
        {
            var elementId = Guid.NewGuid();
            //Arrange
            var patchApprenticeshipFrameworksModel = new PatchApprenticeshipFrameworksModel() { ActionType = messageAction, Id = elementId };
            currentOpportunitiesSegmentModel.Data.Apprenticeships = new Apprenticeships()
            {
               Frameworks = new List<Data.Models.ApprenticeshipFramework>(),
            };
            currentOpportunitiesSegmentModel.Data.Apprenticeships.Frameworks.Add(new Data.Models.ApprenticeshipFramework() { Id = elementId });

            A.CallTo(() => repository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            //Act
            var result = await CurrentOpportunitiesSegmentService.PatchApprenticeshipFrameworksAsync(patchApprenticeshipFrameworksModel, testGuid).ConfigureAwait(false);

            //Asserts
            A.CallTo(() => fakeCurrentOpportunitiesSegmentUtilities.GetReturnStatusForNullElementPatchRequest(A<MessageAction>.Ignored)).MustNotHaveHappened();
            result.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void PatchApprenticeshipStandardsNullTest()
        {
            //Asserts
            Func<Task> f = async () => { await CurrentOpportunitiesSegmentService.PatchApprenticeshipStandardsAsync(null, testGuid).ConfigureAwait(false); };
            f.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task ApprenticeshipStandardsPatchStandardsReturnsNotFoundForMissingSegment()
        {
            //Arrange
            var doNotPatchResult = new CurrentOpportunitiesSegmentPatchStatus() { OkToPatch = false, ReturnStatusCode = HttpStatusCode.NotFound };
            var patchApprenticeshipStandardsModel = new PatchApprenticeshipStandardsModel() { ActionType = MessageAction.Deleted };
            A.CallTo(() => repository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(null as CurrentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCurrentOpportunitiesSegmentUtilities.IsSegementOkToPatch(A<CurrentOpportunitiesSegmentModel>.Ignored, A<long>.Ignored)).Returns(doNotPatchResult);

            //Act
            var result = await CurrentOpportunitiesSegmentService.PatchApprenticeshipStandardsAsync(patchApprenticeshipStandardsModel, testGuid).ConfigureAwait(false);

            //Asserts
            result.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ApprenticeshipStandardsPatchStandardsNullTest()
        {
            //Arrange
            var patchApprenticeshipStandardsModel = new PatchApprenticeshipStandardsModel() { ActionType = MessageAction.Deleted };
            A.CallTo(() => fakeCurrentOpportunitiesSegmentUtilities.GetReturnStatusForNullElementPatchRequest(A<MessageAction>.Ignored)).Returns(HttpStatusCode.AlreadyReported);

            //Act
            var result = await CurrentOpportunitiesSegmentService.PatchApprenticeshipStandardsAsync(patchApprenticeshipStandardsModel, testGuid).ConfigureAwait(false);

            //Asserts
            result.Should().Be(HttpStatusCode.AlreadyReported);
        }

        [Theory]
        [InlineData(MessageAction.Deleted)]
        [InlineData(MessageAction.Published)]
        public async Task ApprenticeshipStandardsPatchTest(MessageAction messageAction)
        {
            var elementId = Guid.NewGuid();
            //Arrange
            var patchApprenticeshipStandardsModel = new PatchApprenticeshipStandardsModel() { ActionType = messageAction, Id = elementId };
            currentOpportunitiesSegmentModel.Data.Apprenticeships = new Apprenticeships()
            {
                Standards = new List<Data.Models.ApprenticeshipStandard>(),
            };
            currentOpportunitiesSegmentModel.Data.Apprenticeships.Standards.Add(new Data.Models.ApprenticeshipStandard() { Id = elementId });

            A.CallTo(() => repository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            //Act
            var result = await CurrentOpportunitiesSegmentService.PatchApprenticeshipStandardsAsync(patchApprenticeshipStandardsModel, testGuid).ConfigureAwait(false);

            //Asserts
            A.CallTo(() => fakeCurrentOpportunitiesSegmentUtilities.GetReturnStatusForNullElementPatchRequest(A<MessageAction>.Ignored)).MustNotHaveHappened();
            result.Should().Be(HttpStatusCode.OK);
        }
    }
}