using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using Microsoft.AspNetCore.Html;

namespace DFC.App.JobProfile.CurrentOpportunities.AutoMapperProfiles
{
    public class CurrentOpportunitiesSegmentModelProfile : Profile
    {
        public CurrentOpportunitiesSegmentModelProfile()
        {
            CreateMap<CurrentOpportunitiesSegmentModel, DocumentViewModel>()
              .ForMember(d => d.Data, s => s.MapFrom(a => a.SegmentData))
              ;

            CreateMap<SegmentData, DocumentDataViewModel>();

            CreateMap<CurrentOpportunitiesSegmentModel, BodyViewModel>()
            .ForMember(d => d.BodyData, s => s.MapFrom(a => a.SegmentData))
            ;

            CreateMap<SegmentData, BodyDataViewModel>();

            CreateMap<Apprenticeship, ApprenticeshipViewModel>();
            CreateMap<Course, CourseViewModel>();

            CreateMap<CurrentOpportunitiesSegmentModel, IndexDocumentViewModel>();
        }
    }
}
