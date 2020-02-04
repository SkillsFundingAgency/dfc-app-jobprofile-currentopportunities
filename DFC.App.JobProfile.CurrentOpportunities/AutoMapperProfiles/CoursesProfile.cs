using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.FindACourseClient;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<Course, Opportunity>()
                .ForMember(d => d.Provider, s => s.MapFrom(f => f.ProviderName))
                .ForMember(d => d.PullDate, s => s.Ignore())
                .ForMember(d => d.URL, s => s.Ignore())
                .ForPath(d => d.Location.Town, s => s.MapFrom(f => f.Location));
        }
    }
}