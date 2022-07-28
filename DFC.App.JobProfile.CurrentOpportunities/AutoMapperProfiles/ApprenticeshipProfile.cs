using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.JobProfile.CurrentOpportunities.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class ApprenticeshipProfile : Profile
    {
        public ApprenticeshipProfile()
        {
            CreateMap<ApprenticeshipVacancyDetails, Vacancy>()
                .ForMember(v => v.URL, opt => opt.MapFrom(s => s.VacancyUrl))
                .ForMember(v => v.ApprenticeshipId, opt => opt.MapFrom(s => s.VacancyReference))
                .ForMember(v => v.WageText, opt => opt.MapFrom(s => s.Wage.WageAdditionalInformation))
                .ForMember(v => v.WageUnit, opt => opt.MapFrom(s => s.Wage.WageUnit))
                .ForMember(v => v.PullDate, opt => opt.Ignore())
                .ForMember(v => v.Location, opt => opt.MapFrom(s => s.Address));
            CreateMap<AddressLocation, Location>()
                .ForMember(v => v.Town, opt => opt.MapFrom(s => GetLocation(s)));
        }

        private string GetLocation(AddressLocation location)
        {
            var locationList = new List<string> { location.AddressLine2, location.AddressLine3, location.AddressLine4 };

            return string.Join(", ", locationList.Where(l => !string.IsNullOrWhiteSpace(l)));
        }
    }
}
