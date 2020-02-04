using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FluentAssertions;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Utilities Tests")]
    public class CurrentOpportunitiesSegmentUtilitiesTests
    {
        private readonly CurrentOpportunitiesSegmentUtilities currentOpportunitiesSegmentUtilities;

        public CurrentOpportunitiesSegmentUtilitiesTests()
        {
            currentOpportunitiesSegmentUtilities = new CurrentOpportunitiesSegmentUtilities();
        }

        [Fact]
        public void GetReturnStatusForNullElementPatchRequestreturnsAlreadyReportedForDeleted()
        {
            //Arrange
            const MessageAction messageAction = MessageAction.Deleted;

            //Act
            var result = currentOpportunitiesSegmentUtilities.GetReturnStatusForNullElementPatchRequest(messageAction);

            //Asserts
            result.Should().Be(HttpStatusCode.AlreadyReported);
        }

        [Fact]
        public void GetReturnStatusForNullElementPatchRequestreturnsNotFoundForPublished()
        {
            //Arrange
            const MessageAction messageAction = MessageAction.Published;

            //Act
            var result = currentOpportunitiesSegmentUtilities.GetReturnStatusForNullElementPatchRequest(messageAction);

            //Asserts
            result.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void PatchJobProfileSocSegementDoesNotExistTest()
        {
            //Arrange
            CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel = null;

            //Act
            var currentOpportunitiesSegmentPatchStatus = currentOpportunitiesSegmentUtilities.IsSegementOkToPatch(currentOpportunitiesSegmentModel, 1);

            //Asserts
            currentOpportunitiesSegmentPatchStatus.ReturnStatusCode.Should().Be(HttpStatusCode.NotFound);
            currentOpportunitiesSegmentPatchStatus.OkToPatch.Should().Be(false);
        }

        [Theory]
        [InlineData(1, 1, HttpStatusCode.AlreadyReported, false)]
        [InlineData(2, 1, HttpStatusCode.AlreadyReported, false)]
        [InlineData(1, 2, HttpStatusCode.OK, true)]
        public void PatchJobProfileSocSegementAlreadyReportedTest(int exitingSequenceNumber, int patchSequenceNumber, HttpStatusCode expectedResponseCode, bool okToPatch)
        {
            //Arrange
            //Arrange
            CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel = new CurrentOpportunitiesSegmentModel()
            {
                SequenceNumber = exitingSequenceNumber,
                Data = new CurrentOpportunitiesSegmentDataModel()
                {
                    Apprenticeships = new Apprenticeships(),
                },
            };

            //Act
            var currentOpportunitiesSegmentPatchStatus = currentOpportunitiesSegmentUtilities.IsSegementOkToPatch(currentOpportunitiesSegmentModel, patchSequenceNumber);

            //Asserts
            currentOpportunitiesSegmentPatchStatus.ReturnStatusCode.Should().Be(expectedResponseCode);
            currentOpportunitiesSegmentPatchStatus.OkToPatch.Should().Be(okToPatch);
        }
    }
}
