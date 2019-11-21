using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class DocumentCoursesViewModel
    {
        public string CourseKeywords { get; set; }

        public IEnumerable<DocumentOpportunityViewModel> Opportunities { get; set; }
    }
}
