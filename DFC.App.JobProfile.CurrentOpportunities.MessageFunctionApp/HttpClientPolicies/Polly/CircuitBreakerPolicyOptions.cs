using System;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.HttpClientPolicies.Polly
{
    public class CircuitBreakerPolicyOptions
    {
        public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);

        public int ExceptionsAllowedBeforeBreaking { get; set; } = 12;
    }
}
