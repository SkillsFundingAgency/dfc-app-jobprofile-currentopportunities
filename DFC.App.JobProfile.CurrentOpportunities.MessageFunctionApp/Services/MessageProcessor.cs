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

        public async Task<HttpStatusCode> ProcessAsync(string message, long sequenceNumber, MessageContentType messageContentType, MessageAction messageAction)
        {
            switch (messageContentType)
            {
                case MessageContentType.SocCodes:
                    {
                        var serviceBusMessage = JsonConvert.DeserializeObject<PatchSocDataServiceBusModel>(message);
                        var patchSocDataModel = mapper.Map<PatchSocDataModel>(serviceBusMessage);
                        patchSocDataModel.MessageAction = messageAction;
                        patchSocDataModel.SequenceNumber = sequenceNumber;

                        return await httpClientService.PatchAsync(patchSocDataModel, "socCodeData").ConfigureAwait(false);
                    }

                case MessageContentType.JobProfile:
                    return await ProcessJobProfileMessageAsync(message, messageAction, sequenceNumber).ConfigureAwait(false);

                default:
                    break;
            }

            return await Task.FromResult(HttpStatusCode.InternalServerError).ConfigureAwait(false);
        }

        private async Task<HttpStatusCode> ProcessJobProfileMessageAsync(string message, MessageAction messageAction, long sequenceNumber)
        {
            var jobProfile = mappingService.MapToSegmentModel(message, sequenceNumber);

            switch (messageAction)
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

                default:
                    throw new ArgumentOutOfRangeException(nameof(messageAction), $"Invalid message action '{messageAction}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageAction)))}'");
            }
        }
    }
}