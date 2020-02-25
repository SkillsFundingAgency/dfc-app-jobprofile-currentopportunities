using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.HttpClientPolicies.Polly
{
    public class RateLimitPolicyOptions
    {
        public int Count { get; set; } = 2;

        public int BackoffPower { get; set; } = 45;  //Tested on local machine lower then this is will still get rate limits.
    }
}
