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
              .ForMember(d => d.Data, s => s.MapFrom(a => a.Data))
              ;

            CreateMap<CurrentOpportunitiesSegmentData, DocumentDataViewModel>();

            CreateMap<CurrentOpportunitiesSegmentModel, BodyViewModel>()
            .ForMember(d => d.Data, s => s.MapFrom(a => a.Data))
            ;

            CreateMap<CurrentOpportunitiesSegmentData, BodyDataViewModel>()
            .ForMember(d => d.CourseSearchUrl, opt => opt.Ignore())
            ;

            CreateMap<Apprenticeship, ApprenticeshipViewModel>();
            CreateMap<Course, CourseViewModel>();

            CreateMap<CurrentOpportunitiesSegmentModel, IndexDocumentViewModel>();
        }
    }
}
