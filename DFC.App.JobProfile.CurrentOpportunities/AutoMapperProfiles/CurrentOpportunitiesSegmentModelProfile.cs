using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using Microsoft.AspNetCore.Html;

namespace DFC.App.CareerPath.AutoMapperProfiles
{
    public class CurrentOpportunitiesSegmentModelProfile : Profile
    {
        public CurrentOpportunitiesSegmentModelProfile()
        {
            CreateMap<CurrentOpportunitiesSegmentModel, DocumentViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Content)))
                ;

            CreateMap<CurrentOpportunitiesSegmentModel, IndexDocumentViewModel>()
                ;
        }
    }
}
