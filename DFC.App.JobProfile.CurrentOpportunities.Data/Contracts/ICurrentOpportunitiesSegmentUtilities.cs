using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface ICurrentOpportunitiesSegmentUtilities
    {
        CurrentOpportunitiesSegmentPatchStatus IsSegementOkToPatch(CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel, long patchSequenceNumber);

        HttpStatusCode GetReturnStatusForNullElementPatchRequest(MessageAction messageAction);
    }
}
