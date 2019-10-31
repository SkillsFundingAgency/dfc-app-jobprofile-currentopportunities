using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Functions
{
    public class SitefinityMessageHandler
    {
        private static readonly string ClassFullName = typeof(SitefinityMessageHandler).FullName;

        [FunctionName("SitefinityMessageHandler")]
        public async Task Run(
            [ServiceBusTrigger("%cms-messages-topic%", "%cms-messages-subscription%", Connection = "service-bus-connection-string")] Message sitefinityMessage,
            [Inject] IMessageProcessor messageProcessor,
            [Inject] ILogger<SitefinityMessageHandler> log)
        {
            if (sitefinityMessage == null)
            {
                throw new ArgumentNullException(nameof(sitefinityMessage));
            }

            sitefinityMessage.UserProperties.TryGetValue("ActionType", out var actionType);
            sitefinityMessage.UserProperties.TryGetValue("CType", out var contentType);
            sitefinityMessage.UserProperties.TryGetValue("Id", out var messageContentId);

            // logger should allow setting up correlation id and should be picked up from message
            log.LogInformation($"{nameof(SitefinityMessageHandler)}: Received message action '{actionType}' for type '{contentType}' with Id: '{messageContentId}': Correlation id {sitefinityMessage.CorrelationId}");

            var message = Encoding.UTF8.GetString(sitefinityMessage?.Body);

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null or empty.", nameof(sitefinityMessage));
            }

            if (!Enum.TryParse<MessageAction>(actionType?.ToString(), out var messageAction))
            {
                throw new ArgumentOutOfRangeException(nameof(actionType), $"Invalid message action '{messageAction}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageAction)))}'");
            }

            if (!Enum.TryParse<MessageContentType>(contentType?.ToString(), out var messageContentType))
            {
                throw new ArgumentOutOfRangeException(nameof(contentType), $"Invalid message content type '{messageContentType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageContentType)))}'");
            }

            var result = await messageProcessor.ProcessAsync(message, sitefinityMessage.SystemProperties.SequenceNumber, messageContentType, messageAction).ConfigureAwait(false);

            switch (result)
            {
                case HttpStatusCode.OK:
                    log.LogInformation($"{ClassFullName}: JobProfile Id: {messageContentId}: Updated segment");
                    break;

                case HttpStatusCode.Created:
                    log.LogInformation($"{ClassFullName}: JobProfile Id: {messageContentId}: Created segment");
                    break;

                case HttpStatusCode.Accepted:
                    log.LogInformation($"{ClassFullName}: JobProfile Id: {messageContentId}: Upserted segment, but Apprenticeship/Course refresh failed");
                    break;

                default:
                    log.LogWarning($"{ClassFullName}: JobProfile Id: {messageContentId}: Segment not Posted: Status: {result}");
                    break;
            }

            throw new Exception("BOOM!");
        }
    }
}