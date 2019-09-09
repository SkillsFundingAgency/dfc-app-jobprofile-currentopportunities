using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class Apprenticeship : IDataModel
    {
        public string Title { get; set; }

        public Uri Uri { get; set; }

        [Display(Name = "Wage")]
        public string Wage { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }
    }
}
