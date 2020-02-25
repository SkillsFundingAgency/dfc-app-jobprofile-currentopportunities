namespace DFC.App.JobProfile.CurrentOpportunities.Data.HttpClientPolicies.Polly
{
    public class CorePolicyOptions
    {
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }

        public RetryPolicyOptions HttpRetry { get; set; }

        public RateLimitPolicyOptions HttpRateLimitRetry { get; set; }
    }
}
