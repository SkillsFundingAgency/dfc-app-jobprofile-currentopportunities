using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class APIAuditRecordAV : IDataModel
    {
        [Required]
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string Etag { get; set; }

        public Guid CorrelationId { get; set; }

        public DateTime AuditDateTime => DateTime.UtcNow;

        public string Request { get; set; }

        public string Response { get; set; }

        public string PartitionKey => $"{AuditDateTime.Year}{AuditDateTime.Month}";
    }
}
