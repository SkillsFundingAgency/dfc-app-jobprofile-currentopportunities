using System;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.APIResponse
{
    public class CurrentOpportunitiesAPIResponse
    {
        public DateTime lastReviewed { get; set; }
        public string jobTitle { get; set; }
        public string titlePrefix { get; set; }
        public string contentTitle { get; set; }
        public Apprenticeships apprenticeships { get; set; }
        public Courses courses { get; set; }
    }
}
