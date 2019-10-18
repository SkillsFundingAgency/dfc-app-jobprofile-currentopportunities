
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.FindACourseClient.Models
{
    public class APIAuditRecordCourse
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
