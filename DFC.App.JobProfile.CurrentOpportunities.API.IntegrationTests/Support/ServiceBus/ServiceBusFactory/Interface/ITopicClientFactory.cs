using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.ServiceBus.ServiceBusFactory.Interface
{
    public interface ITopicClientFactory
    {
        ITopicClient Create(string connectionString);
    }
}
