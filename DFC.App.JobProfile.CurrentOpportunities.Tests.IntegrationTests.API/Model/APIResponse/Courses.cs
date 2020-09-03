using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.APIResponse
{
    public class Courses
    {
        public string courseKeywords { get; set; }
        public List<Opportunity> opportunities { get; set; }
    }
}
