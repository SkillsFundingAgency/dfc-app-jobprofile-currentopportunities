using DFC.App.JobProfile.CurrentOpportunities.Data.Models;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services
{
    public interface IMappingService
    {
        CurrentOpportunitiesSegmentModel MapToSegmentModel(string message, long sequenceNumber);
    }
}