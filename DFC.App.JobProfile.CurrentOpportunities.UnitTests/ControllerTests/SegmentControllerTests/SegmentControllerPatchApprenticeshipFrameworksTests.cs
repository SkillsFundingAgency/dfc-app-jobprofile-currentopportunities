using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.SegmentControllerTests
{
    [Trait("Segment Controller", "Patch Apprenticeship Frameworks Tests")]
    public class SegmentControllerPatchApprenticeshipFrameworksTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task SegmentControllerPatchApprenticeshipFrameworksReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var patchModel = A.Fake<PatchApprenticeshipFrameworksModel>();
            var documentId = Guid.NewGuid();
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.PatchApprenticeshipFrameworksAsync(patchModel, documentId)).Returns(expectedResponse);

            // Act
            var result = await controller.PatchApprenticeshipFrameworks(patchModel, documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.PatchApprenticeshipFrameworksAsync(patchModel, documentId)).MustHaveHappenedOnceExactly();
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)expectedResponse, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task SegmentControllerPatchApprenticeshipFrameworksReturnsNotFound(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NotFound;
            var patchModel = A.Fake<PatchApprenticeshipFrameworksModel>();
            var documentId = Guid.NewGuid();
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.PatchApprenticeshipFrameworksAsync(patchModel, documentId)).Returns(expectedResponse);

            // Act
            var result = await controller.PatchApprenticeshipFrameworks(patchModel, documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.PatchApprenticeshipFrameworksAsync(patchModel, documentId)).MustHaveHappenedOnceExactly();
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)expectedResponse, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task SegmentControllerPatchApprenticeshipFrameworksReturnsBadRequestWhenNullPatchmodel(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.BadRequest;
            PatchApprenticeshipFrameworksModel patchModel = null;
            var documentId = Guid.NewGuid();
            var controller = BuildSegmentController(mediaTypeName);

            // Act
            var result = await controller.PatchApprenticeshipFrameworks(patchModel, documentId).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal((int)expectedResponse, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task SegmentControllerPatchApprenticeshipFrameworksReturnsBadRequestWhenInvalidPatchmodel(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.BadRequest;
            var patchModel = A.Fake<PatchApprenticeshipFrameworksModel>();
            var documentId = Guid.NewGuid();
            var controller = BuildSegmentController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.PatchApprenticeshipFrameworks(patchModel, documentId).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)expectedResponse, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}