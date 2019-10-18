using AutoMapper;
using DFC.App.FindACourseClient.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.AutoMapperProfiles
{
    public class CourseSummaryProfile : Profile
    {
        public CourseSummaryProfile()
        {
            CreateMap<CourseSumary, Opportunity>();
            CreateMap<CourseLocation, Location>();
        }
    }
}
