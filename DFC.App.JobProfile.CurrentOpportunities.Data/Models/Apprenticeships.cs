using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models
{
    public class Apprenticeships
    {
        public IList<ApprenticeshipStandard> Standards { get; set; }

        public IList<ApprenticeshipFramework> Frameworks { get; set; }

        public IEnumerable<Vacancy> Vacancies { get; set; }
    }
}
