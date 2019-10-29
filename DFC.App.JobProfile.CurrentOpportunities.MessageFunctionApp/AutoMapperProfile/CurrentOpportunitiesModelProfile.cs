using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;

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
                .ForMember(d => d.Soc, s => s.MapFrom(a => a.SocCodeData))
                .ForMember(d => d.Apprenticeships.Frameworks, s => s.MapFrom(a => a.SocCodeData.ApprenticeshipFramework))
                .ForMember(d => d.Apprenticeships.Standards, s => s.MapFrom(a => a.SocCodeData.ApprenticeshipStandards))
                ;

            CreateMap<JobProfileMessage, Courses>()
                ;

            CreateMap<Data.ServiceBusModels.ApprenticeshipFramework, Data.Models.ApprenticeshipFramework>();

            CreateMap<Data.ServiceBusModels.ApprenticeshipStandard, Data.Models.ApprenticeshipStandard>();

            CreateMap<SocCodeData, SocData>();

            CreateMap<PatchSocDataModel, SocData>();
        }
    }
}
