using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class Apprenticeships
    {
        public IEnumerable<ApprenticeshipStandard> Standards { get; set; }

        public IEnumerable<ApprenticeshipFramework> Frameworks { get; set; }

        public IEnumerable<Vacancy> Vacancies { get; set; }
    }
}
