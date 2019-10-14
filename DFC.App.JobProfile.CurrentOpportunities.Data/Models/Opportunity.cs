using System;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class Opportunity
    {
        public string Title { get; set; }

        public string CourseId { get; set; }

        public string Provider { get; set; }

        public string StartDate { get; set; }

        public Location Location { get; set; }

        public Uri URL { get; set; }

        public DateTime PullDate { get; set; }
    }
}
