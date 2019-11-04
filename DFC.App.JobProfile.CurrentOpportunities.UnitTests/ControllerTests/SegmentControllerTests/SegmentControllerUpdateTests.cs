﻿using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
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
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            currentOpportunitiesSegmentModel.SequenceNumber = 1;

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(A.Fake<CurrentOpportunitiesSegmentModel>());
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(HttpStatusCode.Created);

            // Act
            var result = await controller.Put(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.OK, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpdateReturnsNotFoundForNonExisting(string mediaTypeName)
        {
            // Arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns((CurrentOpportunitiesSegmentModel)null);

            // Act
            var result = await controller.Put(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotFound, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpdateReturnsBadRequestWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            CurrentOpportunitiesSegmentModel careerPathSegmentModel = null;
            var controller = BuildSegmentController(mediaTypeName);

            // Act
            var result = await controller.Put(careerPathSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpdateReturnsBadRequestWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var careerPathSegmentModel = new CurrentOpportunitiesSegmentModel();
            var controller = BuildSegmentController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.Put(careerPathSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void SegmentControllerUpdateReturnsAlreadyReportedToEarlierSequence(string mediaTypeName)
        {
            // Arrange
            var currentOpportunitiesSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            currentOpportunitiesSegmentModel.SequenceNumber = -1;

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns((CurrentOpportunitiesSegmentModel)null);

            // Act
            var result = await controller.Put(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.AlreadyReported, statusCodeResult.StatusCode);

            controller.Dispose();
        }
    }
}
