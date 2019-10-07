using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class DocumentCoursesViewModel
    {
        public string CourseKeywords { get; set; }

        public IEnumerable<DocumentOpportunityViewModel> Opportunities { get; set; }
    }
}
