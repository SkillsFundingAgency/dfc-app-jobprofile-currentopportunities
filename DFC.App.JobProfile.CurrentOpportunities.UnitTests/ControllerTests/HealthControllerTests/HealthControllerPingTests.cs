using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;
using Xunit;

namespace DFC.App.CareerPath.UnitTests.ControllerTests.HealthControllerTests
{
    [Trait("Health Controller", "ping Tests")]
    public class HealthControllerPingTests : BaseHealthController
    {
        [Fact]
        public void HealthControllerPingReturnsSuccess()
        {
            // Arrange
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            // Act
            var result = controller.Ping();

            // Assert
            var statusResult = Assert.IsType<OkResult>(result);

            A.Equals((int)HttpStatusCode.OK, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
