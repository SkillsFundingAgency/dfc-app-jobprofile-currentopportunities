using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public class CurrentOpportunitiesSegmentUtilities : ICurrentOpportunitiesSegmentUtilities
    {
        public HttpStatusCode GetReturnStatusForNullElementPatchRequest(MessageAction messageAction)
        {
            return messageAction == MessageAction.Deleted ? HttpStatusCode.AlreadyReported : HttpStatusCode.NotFound;
        }

        public CurrentOpportunitiesSegmentPatchStatus IsSegementOkToPatch(CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel, long patchSequenceNumber)
        {
            var currentOpportunitiesSegmentPatchStatus = new CurrentOpportunitiesSegmentPatchStatus() { OkToPatch = true, ReturnStatusCode = HttpStatusCode.OK };

            if (currentOpportunitiesSegmentModel is null)
            {
                currentOpportunitiesSegmentPatchStatus.ReturnStatusCode = HttpStatusCode.NotFound;
                currentOpportunitiesSegmentPatchStatus.OkToPatch = false;
            }
            else if (patchSequenceNumber <= currentOpportunitiesSegmentModel.SequenceNumber)
            {
                currentOpportunitiesSegmentPatchStatus.ReturnStatusCode = HttpStatusCode.AlreadyReported;
                currentOpportunitiesSegmentPatchStatus.OkToPatch = false;
            }

            return currentOpportunitiesSegmentPatchStatus;
        }
    }
}
