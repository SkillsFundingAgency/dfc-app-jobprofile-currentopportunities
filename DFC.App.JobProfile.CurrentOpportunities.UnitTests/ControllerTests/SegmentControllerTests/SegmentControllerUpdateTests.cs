using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.SegmentControllerTests
{
    [Trait("Segment Controller", "Update Tests")]
    public class SegmentControllerUpdateTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpdateReturnsSuccessForExistingItem(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var controller = BuildSegmentController(mediaTypeName);

            var existingModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var modelToUpsert = A.Fake<CurrentOpportunitiesSegmentModel>();

            existingModel.SequenceNumber = 123;
            modelToUpsert.SequenceNumber = existingModel.SequenceNumber + 1;

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingModel);
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await controller.Put(modelToUpsert).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)expectedResponse, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpdateReturnsNotFoundForNonExisting(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NotFound;
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns((CurrentOpportunitiesSegmentModel)null);

            // Act
            var result = await controller.Put(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)expectedResponse, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpdateReturnsBadRequestWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.BadRequest;
            CurrentOpportunitiesSegmentModel careerPathSegmentModel = null;
            var controller = BuildSegmentController(mediaTypeName);

            // Act
            var result = await controller.Put(careerPathSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);

            Assert.Equal((int)expectedResponse, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpdateReturnsBadRequestWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.BadRequest;
            var careerPathSegmentModel = new CurrentOpportunitiesSegmentModel();
            var controller = BuildSegmentController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.Put(careerPathSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal((int)expectedResponse, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpdateReturnsAlreadyReportedToEarlierSequence(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.AlreadyReported;
            var existingModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var modelToUpsert = A.Fake<CurrentOpportunitiesSegmentModel>();

            existingModel.SequenceNumber = 123;
            modelToUpsert.SequenceNumber = existingModel.SequenceNumber - 1;

            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingModel);

            // Act
            var result = await controller.Put(modelToUpsert).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)expectedResponse, statusCodeResult.StatusCode);

            controller.Dispose();
        }
    }
}
