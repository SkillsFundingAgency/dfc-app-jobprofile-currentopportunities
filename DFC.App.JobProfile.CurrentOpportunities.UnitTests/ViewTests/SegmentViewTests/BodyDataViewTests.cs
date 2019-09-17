using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ViewTests
{
    public class BodyDataViewTests : TestBase
    {
        //As a Citizen, I want to the see the current available Apprenticeships
        //A1 - Display of Current opportunities section for available apprenticeships
        [Fact]
        public void DFC114ApprenticeshipFieldsCorrectTest()
        {
            //Arrange
            var model = new BodyViewModel()
            {
                DocumentId = A.Dummy<Guid>(),
                Data = new BodyDataViewModel()
                {
                    JobTitle = A.Dummy<string>(),
                    Apprenticeships = new List<ApprenticeshipViewModel>(),
                    Courses = new List<CourseViewModel>(),
                    CourseSearchUrl = A.Dummy<Uri>(),
                },
            };

            var viewBag = new Dictionary<string, object>();
            var viewRenderer = new RazorEngineRenderer(ViewRootPath);

            //Act
            var viewRenderResponse = viewRenderer.Render(@"Body", model, viewBag);
            Assert.Contains(model.Data.JobTitle, viewRenderResponse, StringComparison.OrdinalIgnoreCase);
        }
    }
}
