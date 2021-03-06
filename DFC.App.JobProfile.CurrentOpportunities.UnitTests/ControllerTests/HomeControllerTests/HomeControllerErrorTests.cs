using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.HomeControllerTests
{
    [Trait("Home Controller", "Error Tests")]
    public class HomeControllerErrorTests : BaseHomeController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public void HomeControllerErrorHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Fake<ErrorViewModel>();
            var controller = BuildHomeController(mediaTypeName);

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }
    }
}
