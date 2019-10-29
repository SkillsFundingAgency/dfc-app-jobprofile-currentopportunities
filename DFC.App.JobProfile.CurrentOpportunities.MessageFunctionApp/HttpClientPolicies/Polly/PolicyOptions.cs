namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.HttpClientPolicies.Polly
{
    public class PolicyOptions
    {
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }

        public RetryPolicyOptions HttpRetry { get; set; }
    }
}
