using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class APIAuditRecord : IDataModel
    {
        public Guid Id => Guid.NewGuid();

        public Guid CorrelationId { get; set; }

        public DateTime AuditDateTime => DateTime.UtcNow;

        public string Request { get; set; }

        public string Response { get; set; }
    }
}
