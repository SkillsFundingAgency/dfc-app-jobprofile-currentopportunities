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
            CreateMap<Result, Course>()
                .ForMember(d => d.CourseId, s => s.MapFrom(f => f.CourseId.ToString()))
                .ForMember(d => d.Title, s => s.MapFrom(f => f.QualificationCourseTitle))
                .ForMember(d => d.LocationDetails, s => s.MapFrom(f => f))
                .ForMember(d => d.StartDate, s => s.MapFrom(f => f.StartDate.ToString()))
                .ForMember(d => d.StartDateLabel, s => s.MapFrom(f => "Start date:"))
                .ForMember(d => d.AttendanceMode, s => s.MapFrom(f => f.DeliveryModeDescription))
                .ForMember(d => d.AttendancePattern, s => s.MapFrom(f => f.VenueAttendancePatternDescription))
                .ForMember(d => d.StudyMode, s => s.MapFrom(f => f.VenueStudyModeDescription))
                .ForMember(d => d.Location, s => s.MapFrom(f => f.VenueTown))
                .ForMember(d => d.Duration, s => s.MapFrom(f => $"{f.DurationValue} {f.DurationUnit.ToString()}"));

            CreateMap<Result, LocationDetails>()
                .ForMember(d => d.Distance, s => s.MapFrom(f => float.Parse(f.Distance ?? "0")))
                .ForMember(d => d.LocationAddress, s => s.MapFrom(f => f.VenueAddress));

            CreateMap<Course, Opportunity>()
                .ForMember(d => d.Provider, s => s.MapFrom(f => f.ProviderName))
                .ForMember(d => d.PullDate, s => s.Ignore())
                .ForMember(d => d.URL, s => s.Ignore())
                .ForPath(d => d.Location.Town, s => s.MapFrom(f => f.Location));
        }
    }
}