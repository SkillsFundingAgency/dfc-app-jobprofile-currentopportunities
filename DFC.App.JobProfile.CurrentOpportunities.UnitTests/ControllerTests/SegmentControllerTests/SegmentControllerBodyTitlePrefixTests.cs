using DFC.App.JobProfile.CurrentOpportunities.Data;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.SegmentControllerTests
{
    [Trait("Segment Controller", "Body Title Prefix Tests")]
    public class SegmentControllerBodyTitlePrefixTests : BaseSegmentController
    {
        private readonly BodyViewModel testBodyViewModel;
        private readonly CurrentOpportunitiesSegmentModel testSegmentModel;

        public SegmentControllerBodyTitlePrefixTests()
        {
            testBodyViewModel = A.Dummy<BodyViewModel>();
            testBodyViewModel.Data = A.Dummy<BodyDataViewModel>();
            testBodyViewModel.Data.Courses = A.Dummy<BodyCoursesViewModel>();

            testSegmentModel = new CurrentOpportunitiesSegmentModel()
            {
                Data = new CurrentOpportunitiesSegmentDataModel()
                {
                },
            };
        }

        [Theory]
        [InlineData("contentTitle", "jobTitle", "contentTitle")]
        [InlineData("", "jobTitle", "jobtitle")]
        public async Task JobTitlePrefixContentTitleTests(string contentTitle, string jobTitle, string expectedTitle)
        {
            //Arrange
            var documentId = Guid.NewGuid();
            testSegmentModel.Data.JobTitle = jobTitle;
            testSegmentModel.Data.ContentTitle = contentTitle;
            testSegmentModel.Data.TitlePrefix = Constants.TitleNoPrefix;

            var controller = BuildSegmentController(MediaTypeNames.Text.Html);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(testSegmentModel);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(testBodyViewModel);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            //Asserts
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.Model as BodyViewModel;
            model.Data.JobTitleWithPrefix.Should().Be(expectedTitle);
            controller.Dispose();
        }

        [Theory]
        [InlineData("plumber", Constants.TitlePrefixAsDefined, "a plumber")]
        [InlineData("actuary", Constants.TitlePrefixAsDefined, "an actuary")]
        [InlineData("actuary", Constants.TitleNoPrefix, "actuary")]
        [InlineData("actuary", Constants.TitlePrefixWithA, "a actuary")]
        [InlineData("actuary", Constants.TitlePrefixWithAn, "an actuary")]
        [InlineData("actuary", Constants.TitleNoTitle, "")]
        public async Task JobTitlePrefixDefaultTests(string jobTitle, string titlePrefix, string expectedTitle)
        {
            //Arrange
            var documentId = Guid.NewGuid();
            testSegmentModel.Data.JobTitle = jobTitle;
            testSegmentModel.Data.TitlePrefix = titlePrefix;

            var controller = BuildSegmentController(MediaTypeNames.Text.Html);

            A.CallTo(() => FakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(testSegmentModel);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<CurrentOpportunitiesSegmentModel>.Ignored)).Returns(testBodyViewModel);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            //Asserts
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.Model as BodyViewModel;
            model.Data.JobTitleWithPrefix.Should().Be(expectedTitle);
            controller.Dispose();
        }
    }
}