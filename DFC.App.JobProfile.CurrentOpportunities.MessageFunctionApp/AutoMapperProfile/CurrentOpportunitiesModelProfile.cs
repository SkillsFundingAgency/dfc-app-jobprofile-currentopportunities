using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using System.Linq;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.AutoMapperProfile
{
    public class CurrentOpportunitiesModelProfile : Profile
    {
        public CurrentOpportunitiesModelProfile()
        {
            CreateMap<JobProfileMessage, CurrentOpportunitiesSegmentModel>()
                .ForMember(d => d.Data, s => s.MapFrom(a => a))
                .ForMember(d => d.DocumentId, s => s.MapFrom(a => a.JobProfileId))
                .ForMember(d => d.Etag, s => s.Ignore())
                ;

            CreateMap<JobProfileMessage, CurrentOpportunitiesSegmentDataModel>()
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.LastModified))
 //               .ForMember(d => d.Soc, s => s.MapFrom(a => a.SocCodeData))
                .ForMember(d => d.Apprenticeships, s => s.MapFrom(a => new Apprenticeships()
                {
                    Frameworks = a.SocCodeData.ApprenticeshipFramework.Select(b => b.Url).ToArray(),
                    Standards = a.SocCodeData.ApprenticeshipStandards.Select(b => b.Url).ToArray(),
                }))
                ;

            CreateMap<JobProfileMessage, Courses>()
                ;

            //CreateMap<Data.ServiceBusModels.ApprenticeshipFramework, Data.Models.ApprenticeshipFramework>();

            //CreateMap<Data.ServiceBusModels.ApprenticeshipStandard, Data.Models.ApprenticeshipStandard>();

            //CreateMap<SocCodeData, SocData>();

            CreateMap<PatchSocDataModel, Apprenticeships>()
                .ForMember(d => d.Frameworks, s => s.MapFrom(a => a.ApprenticeshipFramework.Select(b => b.Url).ToArray()))
                .ForMember(d => d.Standards, s => s.MapFrom(a => a.ApprenticeshipStandards.Select(b => b.Url).ToArray()))
                .ForMember(d => d.Vacancies, s => s.Ignore())
                ;
        }
    }
}
