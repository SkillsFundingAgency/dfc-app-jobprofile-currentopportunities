using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class SegmentData : IDataModel
    {
        public IEnumerable<string> Standards { get; set; }

        public IEnumerable<string> Frameworks { get; set; }

        public string CourseKeywords { get; set; }

        public IEnumerable<Apprenticeship> Apprenticeships { get; set; }

        public IEnumerable<Course> Courses { get; set; }
    }
}
