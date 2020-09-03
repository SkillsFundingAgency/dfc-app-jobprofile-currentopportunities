using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.ServiceBus.ServiceBusFactory.Interface;
using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.ServiceBus.ServiceBusFactory
{
    public class TopicClientFactory : ITopicClientFactory
    {
        public ITopicClient Create(string connectionString)
        {
            return new TopicClient(new ServiceBusConnectionStringBuilder(connectionString));
        }
    }
}
