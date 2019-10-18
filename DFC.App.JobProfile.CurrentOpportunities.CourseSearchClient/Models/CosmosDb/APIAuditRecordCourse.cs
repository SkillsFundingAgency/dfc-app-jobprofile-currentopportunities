
using DFC.App.FindACourseClient.Contracts.CosmosDb;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.FindACourseClient.Models.CosmosDb
{
    public class APIAuditRecordCourse : IDataModel
    {
        [Required]
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string Etag { get; set; }

        public Guid CorrelationId { get; set; }

        public DateTime AuditDateTime => DateTime.UtcNow;

        public object Request { get; set; }

        public object Response { get; set; }

        public string PartitionKey => $"{AuditDateTime.Year}{AuditDateTime.Month}";
    }
}
