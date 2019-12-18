using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.FindACourseClient.Models;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
           // CreateMap<CourseSumary, Opportunity>()
           //.ForMember(o => o.URL, opt => opt.Ignore())
           //.ForMember(o => o.PullDate, opt => opt.Ignore());
           // CreateMap<CourseLocation, Location>();
        }
    }
}
