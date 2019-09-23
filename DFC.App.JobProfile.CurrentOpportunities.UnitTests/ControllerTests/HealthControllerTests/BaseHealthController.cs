using DFC.App.JobProfile.CurrentOpportunities.Controllers;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ControllerTests.HealthControllerTests
{
    public abstract class BaseHealthController
    {
        public BaseHealthController()
        {
            FakeCurrentOpportunitiesSegmentService = A.Fake<ICurrentOpportunitiesSegmentService>();
            FakeLogger = A.Fake<ILogger<HealthController>>();
        }

        protected ICurrentOpportunitiesSegmentService FakeCurrentOpportunitiesSegmentService { get; }

        protected ILogger<HealthController> FakeLogger { get; }

        protected HealthController BuildHealthController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new HealthController(FakeLogger, FakeCurrentOpportunitiesSegmentService)
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