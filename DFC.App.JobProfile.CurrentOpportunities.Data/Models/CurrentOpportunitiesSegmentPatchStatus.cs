using System.Net;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class CurrentOpportunitiesSegmentPatchStatus
    {
        public HttpStatusCode ReturnStatusCode { get; set; }

        public bool OkToPatch { get; set; }
    }
}
