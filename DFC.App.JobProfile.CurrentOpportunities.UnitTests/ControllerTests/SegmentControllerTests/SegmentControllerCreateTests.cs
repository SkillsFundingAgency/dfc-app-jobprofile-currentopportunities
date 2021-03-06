﻿using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.SegmentControllerTests
{
    [Trait("Segment Controller", "Create Tests")]
    public class SegmentControllerCreateTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task SegmentControllerCreateReturnsSuccessForNewItem(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns((CurrentOpportunitiesSegmentModel)null);
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await controller.Post(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)expectedResponse, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task SegmentControllerCreateReturnsAlreadyReportedForExisting(string mediaTypeName)
        {
            // Arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(A.Fake<CurrentOpportunitiesSegmentModel>());

            // Act
            var result = await controller.Post(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.AlreadyReported, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task SegmentControllerCreateReturnsBadRequestWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            CurrentOpportunitiesSegmentModel careerPathSegmentModel = null;
            var controller = BuildSegmentController(mediaTypeName);

            // Act
            var result = await controller.Post(careerPathSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);

            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task SegmentControllerCreateReturnsBadRequestWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var careerPathSegmentModel = new CurrentOpportunitiesSegmentModel();
            var controller = BuildSegmentController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.Post(careerPathSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}