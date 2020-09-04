using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.ServiceBus
{
    public interface IServiceBusSupport
    {
        Task SendMessage(Message message);
    }
}
