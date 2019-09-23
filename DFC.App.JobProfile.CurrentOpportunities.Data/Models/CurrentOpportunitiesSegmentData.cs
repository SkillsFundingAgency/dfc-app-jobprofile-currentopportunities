using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class CurrentOpportunitiesSegmentData : IDataModel
    {
        public DateTime Updated { get; set; }

        public string JobTitle { get; set; }

        public DateTime LastReviewed { get; set; }

        public IEnumerable<string> Standards { get; set; }

        public IEnumerable<string> Frameworks { get; set; }

        public string CourseKeywords { get; set; }

        public IEnumerable<Apprenticeship> Apprenticeships { get; set; }

        public IEnumerable<Course> Courses { get; set; }
    }
}
