using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.ViewModels
{
    public class ApprenticeshipViewModel
    {
        public string Title { get; set; }

        public string ApprenticeshipId { get; set; }

        public string WageUnit { get; set; }

        public string WageText { get; set; }

        public string LocationTown { get; set; }

        public string LocationPostCode { get; set; }
    }
}
