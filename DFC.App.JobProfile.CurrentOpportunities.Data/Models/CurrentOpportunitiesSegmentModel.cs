using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class CurrentOpportunitiesSegmentModel : IDataModel
    {
        public const string SegmentName = "CurrentOpportunities";

        [Required]
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string Etag { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        public string PartitionKey => SocLevelTwo;

        [Required]
        public DateTime? LastReviewed { get; set; }

        public CurrentOpportunitiesSegmentDataModel Data { get; set; }
    }
}
