using System;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.APIResponse
{
    public class Opportunity
    {
        public string title { get; set; }
        public string courseId { get; set; }
        public string runId { get; set; }
        public string provider { get; set; }
        public string startDate { get; set; }
        public OpportunityLocation location { get; set; }
        public string url { get; set; }
        public DateTime pullDate { get; set; }
    }
}
