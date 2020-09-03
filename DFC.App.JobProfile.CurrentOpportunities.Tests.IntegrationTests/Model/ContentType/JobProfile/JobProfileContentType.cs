using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.ContentType.JobProfile
{
    public class JobProfileContentType
    {
        public string JobProfileId { get; set; }

        public string Title { get; set; }

        public string DynamicTitlePrefix { get; set; }

        public string AlternativeTitle { get; set; }

        public string Overview { get; set; }

        public string SocLevelTwo { get; set; }

        public string UrlName { get; set; }

        public string DigitalSkillsLevel { get; set; }

        public List<Restriction> Restrictions { get; set; }

        public string OtherRequirements { get; set; }

        public string CareerPathAndProgression { get; set; }

        public string CourseKeywords { get; set; }

        public double? MinimumHours { get; set; }

        public double? MaximumHours { get; set; }

        public double? SalaryStarter { get; set; }

        public double? SalaryExperienced { get; set; }

        public List<WorkingPattern> WorkingPattern { get; set; }

        public List<WorkingPatternDetail> WorkingPatternDetails { get; set; }

        public List<WorkingHoursDetail> WorkingHoursDetails { get; set; }

        public List<HiddenAlternativeTitle> HiddenAlternativeTitle { get; set; }

        public List<JobProfileSpecialism> JobProfileSpecialism { get; set; }

        public bool? IsImported { get; set; }

        public HowToBecomeData HowToBecomeData { get; set; }

        public WhatYouWillDoData WhatYouWillDoData { get; set; }

        public SocCodeData SocCodeData { get; set; }

        public List<RelatedCareersData> RelatedCareersData { get; set; }

        public List<SocSkillsMatrixData> SocSkillsMatrixData { get; set; }

        public List<JobProfileCategory> JobProfileCategories { get; set; }

        public DateTime? LastModified { get; set; }

        public string CanonicalName { get; set; }

        public string WidgetContentTitle { get; set; }

        public bool? IncludeInSitemap { get; set; }
    }
}
