using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Models
{
    public class FeedRefreshResponseModel
    {
        public int NumberPulled { get; set; }

        public string RequestErrorMessage { get; set; }
    }
}
