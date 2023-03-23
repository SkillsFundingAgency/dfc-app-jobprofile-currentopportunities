﻿using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class CurrentOpportunitiesSegmentModelProfile : Profile
    {
        public CurrentOpportunitiesSegmentModelProfile()
        {
            CreateMap<CurrentOpportunitiesSegmentModel, DocumentViewModel>();
            CreateMap<CurrentOpportunitiesSegmentDataModel, DocumentDataViewModel>()
                .ForMember(d => d.JobTitleWithPrefix, opt => opt.Ignore());
            CreateMap<Apprenticeships, DocumentApprenticeshipsViewModel>();
            CreateMap<Vacancy, DocumentVacancyViewModel>();
            CreateMap<Courses, DocumentCoursesViewModel>()
                .ForMember(d => d.CourseSearchUrl, opt => opt.Ignore());
            CreateMap<Opportunity, DocumentOpportunityViewModel>();

            CreateMap<CurrentOpportunitiesSegmentModel, BodyViewModel>();
            CreateMap<CurrentOpportunitiesSegmentDataModel, BodyDataViewModel>()
                .ForMember(d => d.JobTitleWithPrefix, opt => opt.Ignore());

            CreateMap<Apprenticeships, BodyApprenticeshipsViewModel>();
            CreateMap<Vacancy, BodyVacancyViewModel>();
            CreateMap<Courses, BodyCoursesViewModel>()
                .ForMember(d => d.CourseSearchUrl, opt => opt.Ignore());

            CreateMap<Opportunity, BodyOpportunityViewModel>();
            CreateMap<Location, LocationViewModel>()
                .ForMember(d => d.PostCode, opt => opt.Ignore());

            CreateMap<CurrentOpportunitiesSegmentModel, IndexDocumentViewModel>();

            CreateMap<PatchApprenticeshipStandardsModel, ApprenticeshipStandard>();

            CreateMap<PatchApprenticeshipFrameworksModel, ApprenticeshipFramework>();

            CreateMap<Data.Models.CurrentOpportunitiesSegmentModel, Data.ServiceBusModels.RefreshJobProfileSegmentServiceBusModel>()
                .ForMember(d => d.JobProfileId, s => s.MapFrom(a => a.DocumentId))
                .ForMember(d => d.Segment, s => s.MapFrom(a => Data.Models.CurrentOpportunitiesSegmentDataModel.SegmentName))
                ;
        }
    }
}
