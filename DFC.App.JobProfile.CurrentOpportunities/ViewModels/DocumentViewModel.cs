using Microsoft.AspNetCore.Html;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class DocumentViewModel
    {
        [Display(Name = "Document Id")]
        public Guid? DocumentId { get; set; }

        [Display(Name = "Canonical Name")]
        public string CanonicalName { get; set; }

        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        public DocumentDataViewModel Data { get; set; }

        [Display(Name = "Last Reviewed")]
        public DateTime LastReviewed { get; set; }
    }
}
