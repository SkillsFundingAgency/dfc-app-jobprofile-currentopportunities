using DFC.App.JobProfile.CurrentOpportunities.AVService.Helpers;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using Microsoft.Extensions.Logging;
using System;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService
{
    public class AuditService : IAuditService
    {
        private readonly ICosmosRepository<APIAuditRecordAV> auditRepository;
        private readonly ILogger<IAuditService> logger;

        public AuditService(ICosmosRepository<APIAuditRecordAV> auditRepository, ILogger<IAuditService> logger)
        {
            this.auditRepository = auditRepository;
            this.logger = logger;
        }

        public void CreateAudit(object request, object response, Guid? correlationId = null)
        {
            var auditRecord = new APIAuditRecordAV
            {
                DocumentId = Guid.NewGuid(),
                CorrelationId = correlationId ?? Guid.NewGuid(),
                Request = request,
                Response = response,
            };

            TaskHelper.ExecuteNoWait(() => auditRepository.UpsertAsync(auditRecord).ConfigureAwait(false), ex => logger.LogError(ex, $"Failed to create audit message"));
        }
    }
}