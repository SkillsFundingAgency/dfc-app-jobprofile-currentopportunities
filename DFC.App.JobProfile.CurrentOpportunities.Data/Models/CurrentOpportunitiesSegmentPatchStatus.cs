using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class CurrentOpportunitiesSegmentPatchStatus
    {
        public HttpStatusCode ReturnStatusCode { get; set; }

        public bool OkToPatch { get; set; }
    }
}
