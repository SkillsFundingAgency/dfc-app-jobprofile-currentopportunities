using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Net.Mime;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.HealthControllerTests
{
    [Trait("Health Controller", "Health Tests")]
    public class HealthControllerHealthTests : BaseHealthController
    {
        [Theory]
        [InlineData (HealthServiceState.Green, HttpStatusCode.OK)]
        [InlineData(HealthServiceState.Red, HttpStatusCode.BadGateway)]
        public async void HealthControllerHealthReturnsSuccessWhenhealthy(HealthServiceState healthServiceState, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var returnedServiceHealthStatus = new ServiceHealthStatus() { HealthServiceState = healthServiceState };
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetCurrentHealthStatusAsync()).Returns(returnedServiceHealthStatus);
            A.CallTo(() => FakeAVAPIService.GetCurrentHealthStatusAsync()).Returns(returnedServiceHealthStatus);

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetCurrentHealthStatusAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeAVAPIService.GetCurrentHealthStatusAsync()).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HealthViewModel>(statusResult.Value);

            A.Equals((int)expectedStatusCode, statusResult.StatusCode);

            model.HealthItems.Count.Should().Be(2);
            controller.Dispose();
        }
    }
}
