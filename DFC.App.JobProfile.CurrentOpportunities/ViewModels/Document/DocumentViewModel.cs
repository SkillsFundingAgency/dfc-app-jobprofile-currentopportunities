using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class DocumentViewModel
    {
        [Display(Name = "Document Id")]
        public Guid? DocumentId { get; set; }

        [Display(Name = "Canonical Name")]
        public string CanonicalName { get; set; }

        [Display(Name = "Sequence Number")]
        public long SequenceNumber { get; set; }

        public DocumentDataViewModel Data { get; set; }
    }
}
