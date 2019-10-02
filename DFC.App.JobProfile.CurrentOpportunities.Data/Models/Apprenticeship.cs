using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class Apprenticeship : IDataModel
    {
        public string Title { get; set; }

        public string ApprenticeshipId { get; set; }

        public string WageUnit { get; set; }

        public string WageText { get; set; }

        public Location Location { get; set; }
    }
}
