using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using Newtonsoft.Json;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services
{
    public class MappingService : IMappingService
    {
        private readonly IMapper mapper;

        public MappingService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public CurrentOpportunitiesSegmentModel MapToSegmentModel(string message, long sequenceNumber)
        {
            var fullJobProfileMessage = JsonConvert.DeserializeObject<JobProfileMessage>(message);
            var fullJobProfile = mapper.Map<CurrentOpportunitiesSegmentModel>(fullJobProfileMessage);

            fullJobProfile.SequenceNumber = sequenceNumber;
            fullJobProfile.Data.Apprenticeships = mapper.Map<Apprenticeships>(fullJobProfileMessage.SocCodeData);
            fullJobProfile.Data.Courses = mapper.Map<Courses>(fullJobProfileMessage);

            return fullJobProfile;
        }
    }
}