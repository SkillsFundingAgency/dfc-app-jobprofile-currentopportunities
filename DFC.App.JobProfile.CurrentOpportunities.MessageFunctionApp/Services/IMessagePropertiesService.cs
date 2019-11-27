using Microsoft.Azure.ServiceBus;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services
{
    public interface IMessagePropertiesService
    {
        long GetSequenceNumber(Message message);
    }
}
