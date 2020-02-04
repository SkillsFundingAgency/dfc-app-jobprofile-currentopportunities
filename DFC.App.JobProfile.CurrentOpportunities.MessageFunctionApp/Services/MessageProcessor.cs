using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels.PatchModels;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IMapper mapper;
        private readonly IHttpClientService httpClientService;
        private readonly IMappingService mappingService;

        public MessageProcessor(IMapper mapper, IHttpClientService httpClientService, IMappingService mappingService)
        {
            this.mapper = mapper;
            this.httpClientService = httpClientService;
            this.mappingService = mappingService;
        }

        public async Task<HttpStatusCode> ProcessAsync(string message, long sequenceNumber, MessageContentType messageContentType, MessageAction actionType)
        {
            switch (messageContentType)
            {
                case MessageContentType.JobProfileSoc:
                    var serviceBusJobProfileSocMessage = JsonConvert.DeserializeObject<PatchJobProfileSocServiceBusModel>(message);
                    var patchSocDataModel = mapper.Map<PatchJobProfileSocModel>(serviceBusJobProfileSocMessage);
                    patchSocDataModel.ActionType = actionType;
                    patchSocDataModel.SequenceNumber = sequenceNumber;

                    return await httpClientService.PatchAsync(patchSocDataModel, $"{messageContentType}").ConfigureAwait(false);

                case MessageContentType.ApprenticeshipFrameworks:
                    var serviceBusApprenticeshipFrameworksMessage = JsonConvert.DeserializeObject<PatchApprenticeshipFrameworksServiceBusModel>(message);
                    var patchApprenticeshipFrameworksModel = mapper.Map<PatchApprenticeshipFrameworksModel>(serviceBusApprenticeshipFrameworksMessage);
                    patchApprenticeshipFrameworksModel.ActionType = actionType;
                    patchApprenticeshipFrameworksModel.SequenceNumber = sequenceNumber;

                    return await httpClientService.PatchAsync(patchApprenticeshipFrameworksModel, $"{messageContentType}").ConfigureAwait(false);

                case MessageContentType.ApprenticeshipStandards:
                    var serviceBusApprenticeshipStandardsMessage = JsonConvert.DeserializeObject<PatchApprenticeshipStandardsServiceBusModel>(message);
                    var patchApprenticeshipStandardsModel = mapper.Map<PatchApprenticeshipStandardsModel>(serviceBusApprenticeshipStandardsMessage);
                    patchApprenticeshipStandardsModel.ActionType = actionType;
                    patchApprenticeshipStandardsModel.SequenceNumber = sequenceNumber;

                    return await httpClientService.PatchAsync(patchApprenticeshipStandardsModel, $"{messageContentType}").ConfigureAwait(false);

                case MessageContentType.JobProfile:
                    return await ProcessJobProfileMessageAsync(message, actionType, sequenceNumber).ConfigureAwait(false);

                default:
                    throw new ArgumentOutOfRangeException(nameof(messageContentType), $"Unexpected sitefinity content type '{messageContentType}'");
            }
        }

        private async Task<HttpStatusCode> ProcessJobProfileMessageAsync(string message, MessageAction actionType, long sequenceNumber)
        {
            var jobProfile = mappingService.MapToSegmentModel(message, sequenceNumber);

            switch (actionType)
            {
                case MessageAction.Draft:
                case MessageAction.Published:
                    var result = await httpClientService.PutAsync(jobProfile).ConfigureAwait(false);
                    if (result == HttpStatusCode.NotFound)
                    {
                        return await httpClientService.PostAsync(jobProfile).ConfigureAwait(false);
                    }

                    return result;

                case MessageAction.Deleted:
                    return await httpClientService.DeleteAsync(jobProfile.DocumentId).ConfigureAwait(false);
            }

            return HttpStatusCode.InternalServerError;
        }
    }
}