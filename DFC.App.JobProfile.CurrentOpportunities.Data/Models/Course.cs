﻿using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using System;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class Course : IDataModel
    {
        public string Title { get; set; }

        public Uri Uri { get; set; }

        public string Provider { get; set; }

        public string Start { get; set; }

        public string Location { get; set; }
    }
}