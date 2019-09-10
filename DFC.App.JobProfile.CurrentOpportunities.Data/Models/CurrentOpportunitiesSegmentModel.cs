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

        public string JobTitle { get; set; }

        public SegmentData SegmentData { get; set; }

        public DateTime LastReviewed { get; set; }
    }
}
