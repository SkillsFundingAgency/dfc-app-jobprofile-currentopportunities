﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class CourseViewModel
    {
        public string Title { get; set; }

        public Uri Uri { get; set; }

        [Display(Name = "Provider")]
        public string Provider { get; set; }

        [Display(Name = "Start Date")]
        public DateTime Start { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }
    }
}