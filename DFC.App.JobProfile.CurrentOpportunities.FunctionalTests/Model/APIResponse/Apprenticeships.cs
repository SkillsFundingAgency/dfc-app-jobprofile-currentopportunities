using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.ContentType.JobProfile;
using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.APIResponse
{
    public class Apprenticeships
    {
        public List<ApprenticeshipStandard> standards { get; set; }

        public List<ApprenticeshipFramework> frameworks { get; set; }

        public List<Vacancy> vacancies { get; set; }
    }
}
