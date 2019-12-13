using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services
{
    public interface IMessagePropertiesService
    {
        long GetSequenceNumber(Message message);
    }
}
