using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class Opportunity
    {
        public string Title { get; set; }

        public string CourseId { get; set; }

        public string RunId { get; set; }

        public string TLevelId { get; set; }

        public string TLevelLocationId { get; set; }

        public string Provider { get; set; }

        public string StartDate { get; set; }

        public bool FlexibleStartDate { get; set; }

        public Location Location { get; set; }

        public Uri URL { get; set; }

        public DateTime PullDate { get; set; } = DateTime.Now;
    }
}
