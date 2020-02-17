using System;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface IAuditService
    {
        //Fire and forget audit
        void CreateAudit(object request, object response, Guid? correlationId = null);
    }
}