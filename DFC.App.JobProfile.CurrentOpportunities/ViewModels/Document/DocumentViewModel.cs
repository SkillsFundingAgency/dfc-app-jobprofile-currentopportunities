using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class DocumentViewModel
    {
        [Display(Name = "Document Id")]
        public Guid? DocumentId { get; set; }

        public string Etag { get; set; }

        [Display(Name = "Canonical Name")]
        public string CanonicalName { get; set; }

        [Display(Name = "Last Reviewed")]
        public DateTime? LastReviewed { get; set; }

        [Display(Name = "SOC Level two")]
        public string SocLevelTwo { get; set; }

        [Display(Name = "Partition Key")]
        public string PartitionKey { get; set; }

        public DocumentDataViewModel Data { get; set; }
    }
}
