using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class CurrentOpportunitiesSegmentModel : IDataModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public int PartitionKey => Created.Second;

        public DateTime Updated { get; set; }

        public CurrentOpportunitiesSegmentData Data { get; set; }
    }
}
