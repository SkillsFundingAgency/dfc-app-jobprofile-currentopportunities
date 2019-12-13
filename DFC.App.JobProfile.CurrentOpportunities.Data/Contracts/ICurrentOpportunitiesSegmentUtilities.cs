using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System.Net;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface ICurrentOpportunitiesSegmentUtilities
    {
        CurrentOpportunitiesSegmentPatchStatus IsSegementOkToPatch(CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel, long patchSequenceNumber);

        HttpStatusCode GetReturnStatusForNullElementPatchRequest(MessageAction messageAction);
    }
}
