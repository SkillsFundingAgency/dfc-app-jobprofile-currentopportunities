using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class BodyCoursesViewModel
    {
        public string CourseKeywords { get; set; }

        public Uri CourseSearchUrl { get; set; }

        public IEnumerable<BodyOpportunityViewModel> Opportunities { get; set; }
    }
}
