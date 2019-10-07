using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class DocumentOpportunityViewModel
    {
        public string Title { get; set; }

        public string CourseId { get; set; }

        public string Provider { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public LocationViewModel Location { get; set; }

        public Uri URL { get; set; }

        [Display(Name = "Pull Date")]
        public DateTime PullDate { get; set; }
    }
}
