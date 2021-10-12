using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class DocumentCoursesViewModel
    {
        public string CourseKeywords { get; set; }

        public Uri CourseSearchUrl { get; set; }

        public IEnumerable<DocumentOpportunityViewModel> Opportunities { get; set; }
    }
}
