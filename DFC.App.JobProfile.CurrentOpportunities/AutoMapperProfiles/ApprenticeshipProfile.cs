using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;

namespace DFC.App.JobProfile.CurrentOpportunities.AutoMapperProfiles
{
    public class ApprenticeshipProfile : Profile
    {
        public ApprenticeshipProfile()
        {
            CreateMap<ApprenticeshipVacancyDetails, Vacancy>();
            CreateMap<AddressLocation, Location>();
        }
    }
}
