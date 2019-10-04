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
            CreateMap<CurrentOpportunitiesSegmentModel, DocumentViewModel>();
            CreateMap<CurrentOpportunitiesSegmentDataModel, DocumentDataViewModel>();
            CreateMap<CurrentOpportunitiesSegmentModel, BodyViewModel>();
            CreateMap<CurrentOpportunitiesSegmentDataModel, BodyDataViewModel>()
            .ForMember(d => d.CourseSearchUrl, opt => opt.Ignore());

            CreateMap<Vacancy, ApprenticeshipViewModel>();
            CreateMap<Opportunity, CourseViewModel>();
            CreateMap<Location, LocationViewModel>();
            CreateMap<CurrentOpportunitiesSegmentModel, IndexDocumentViewModel>();
        }
    }
}
