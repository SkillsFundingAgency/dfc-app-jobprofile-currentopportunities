using DFC.App.JobProfile.CurrentOpportunities.Controllers;
using DFC.App.JobProfile.CurrentOpportunities.Data.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Mime;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.SegmentControllerTests
{
    public abstract class BaseSegmentController
    {
        public BaseSegmentController()
        {
            FakeLogger = A.Fake<ILogger<SegmentController>>();
            FakeCareerPathSegmentService = A.Fake<ICurrentOpportunitiesSegmentService>();
            FakeMapper = A.Fake<AutoMapper.IMapper>();
            FakeCourseSearchConfig = new CourseSearchConfig() { CourseSearchUrl = new System.Uri("http://test") };
        }

        public static IEnumerable<object[]> HtmlMediaTypes => new List<object[]>
        {
            new string[] { "*/*" },
            new string[] { MediaTypeNames.Text.Html },
        };

        protected ILogger<SegmentController> FakeLogger { get; }

        protected ICurrentOpportunitiesSegmentService FakeCareerPathSegmentService { get; }

        protected AutoMapper.IMapper FakeMapper { get; }

        protected CourseSearchConfig FakeCourseSearchConfig { get; }

        protected SegmentController BuildSegmentController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new SegmentController(FakeLogger, FakeCareerPathSegmentService, FakeMapper, FakeCourseSearchConfig)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}
