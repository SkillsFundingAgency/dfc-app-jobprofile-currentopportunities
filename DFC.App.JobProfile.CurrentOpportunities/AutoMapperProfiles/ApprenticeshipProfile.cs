﻿using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;

namespace DFC.App.JobProfile.CurrentOpportunities.AutoMapperProfiles
{
    public class ApprenticeshipProfile : Profile
    {
        public ApprenticeshipProfile()
        {
            CreateMap<ApprenticeshipVacancyDetails, Vacancy>()
                .ForMember(v => v.URL, opt => opt.MapFrom(s => s.VacancyUrl))
                .ForMember(v => v.ApprenticeshipId, opt => opt.MapFrom(s => s.VacancyReference))
                .ForMember(v => v.PullDate, opt => opt.Ignore());
            CreateMap<AddressLocation, Location>();
        }
    }
}
