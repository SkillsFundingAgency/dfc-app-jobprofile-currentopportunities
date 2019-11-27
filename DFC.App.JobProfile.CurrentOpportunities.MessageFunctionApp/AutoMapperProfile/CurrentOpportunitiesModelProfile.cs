using AutoMapper;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.AutoMapperProfile
{
    [ExcludeFromCodeCoverage]
    public class CurrentOpportunitiesModelProfile : Profile
    {
        public CurrentOpportunitiesModelProfile()
        {
            CreateMap<Data.ServiceBusModels.JobProfileMessage, Data.Models.CurrentOpportunitiesSegmentModel>()
                .ForMember(d => d.Data, s => s.MapFrom(a => a))
                .ForMember(d => d.DocumentId, s => s.MapFrom(a => a.JobProfileId))
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.SequenceNumber, s => s.Ignore())
                ;

            CreateMap<Data.ServiceBusModels.JobProfileMessage, Data.Models.CurrentOpportunitiesSegmentDataModel>()
                .ForMember(d => d.JobTitle, s => s.MapFrom(a => a.Title))
                .ForMember(d => d.TitlePrefix, s => s.MapFrom(a => a.DynamicTitlePrefix))
                .ForMember(d => d.ContentTitle, s => s.MapFrom(a => a.WidgetContentTitle))
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.LastModified))
                .ForMember(d => d.Apprenticeships, s => s.MapFrom(a => a.SocCodeData))
                .ForMember(d => d.Courses, s => s.MapFrom(a => new Data.Models.Courses()
                {
                    CourseKeywords = a.CourseKeywords,
                    Opportunities = new List<Data.Models.Opportunity>(),
                }))
                ;

            CreateMap<Data.ServiceBusModels.JobProfileMessage, Data.Models.Courses>()
                .ForMember(d => d.Opportunities, s => s.MapFrom(a => new List<Data.Models.Opportunity>()))
                ;

            CreateMap<Data.ServiceBusModels.SocCodeData, Data.Models.Apprenticeships>()
                .ForMember(d => d.Frameworks, s => s.MapFrom(a => a.ApprenticeshipFramework))
                .ForMember(d => d.Standards, s => s.MapFrom(a => a.ApprenticeshipStandards))
                .ForMember(d => d.Vacancies, s => s.MapFrom(a => new List<Data.Models.Vacancy>()))
                ;

            CreateMap<Data.ServiceBusModels.ApprenticeshipFramework, Data.Models.ApprenticeshipFramework>();

            CreateMap<Data.ServiceBusModels.ApprenticeshipStandard, Data.Models.ApprenticeshipStandard>();

            CreateMap<Data.ServiceBusModels.PatchModels.PatchJobProfileSocServiceBusModel, Data.Models.PatchModels.PatchJobProfileSocModel>()
                .ForMember(d => d.ActionType, s => s.Ignore())
                .ForMember(d => d.SequenceNumber, s => s.Ignore())
                ;

            CreateMap<Data.ServiceBusModels.PatchModels.PatchApprenticeshipFrameworksServiceBusModel, Data.Models.PatchModels.PatchApprenticeshipFrameworksModel>()
                .ForMember(d => d.ActionType, s => s.Ignore())
                .ForMember(d => d.SequenceNumber, s => s.Ignore())
                ;

            CreateMap<Data.ServiceBusModels.PatchModels.PatchApprenticeshipStandardsServiceBusModel, Data.Models.PatchModels.PatchApprenticeshipStandardsModel>()
                .ForMember(d => d.ActionType, s => s.Ignore())
                .ForMember(d => d.SequenceNumber, s => s.Ignore())
                ;
        }
    }
}
