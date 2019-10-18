using AutoMapper;
using DFC.App.FindACourseClient.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.AutoMapperProfiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<CourseSumary, Opportunity>();
            CreateMap<CourseLocation, Location>();
            CreateMap<FeedRefreshResponseModel, FeedRefreshResponseViewModel>();
        }
    }
}
