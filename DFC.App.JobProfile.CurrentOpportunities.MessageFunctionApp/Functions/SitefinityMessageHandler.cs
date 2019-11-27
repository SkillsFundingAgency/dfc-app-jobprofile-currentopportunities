using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Functions
{
    public static class SitefinityMessageHandler
    {
        private static readonly string ClassFullName = typeof(SitefinityMessageHandler).FullName;

        [FunctionName("SitefinityMessageHandler")]
        public static async Task Run(
            [ServiceBusTrigger("%cms-messages-topic%", "%cms-messages-subscription%", Connection = "service-bus-connection-string")] Message sitefinityMessage,
            [Inject] IMessageProcessor messageProcessor,
            [Inject] IMessagePropertiesService messagePropertiesService,
            ILogger log)
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

            if (!Enum.IsDefined(typeof(MessageAction), actionType?.ToString()))
            {
                throw new ArgumentOutOfRangeException(nameof(actionType), $"Invalid message action '{actionType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageAction)))}'");
            }

            if (contentType.ToString().Contains("-", StringComparison.OrdinalIgnoreCase))
            {
                var contentTypeString = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(contentType.ToString());
                contentType = contentTypeString.Replace("-", string.Empty, true, CultureInfo.InvariantCulture);
            }

            if (!Enum.IsDefined(typeof(MessageContentType), contentType?.ToString()))
            {
                throw new ArgumentOutOfRangeException(nameof(contentType), $"Invalid message content type '{contentType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageContentType)))}'");
            }

            var messageAction = Enum.Parse<MessageAction>(actionType?.ToString());
            var messageContentType = Enum.Parse<MessageContentType>(contentType?.ToString());
            var sequenceNumber = messagePropertiesService.GetSequenceNumber(sitefinityMessage);

            var result = await messageProcessor.ProcessAsync(message, sequenceNumber, messageContentType, messageAction).ConfigureAwait(false);

            switch (result)
            {
                case HttpStatusCode.OK:
                    log.LogInformation($"{ClassFullName}: JobProfile Id: {messageContentId}: Updated segment");
                    break;

                case HttpStatusCode.Created:
                    log.LogInformation($"{ClassFullName}: JobProfile Id: {messageContentId}: Created segment");
                    break;

                case HttpStatusCode.AlreadyReported:
                    log.LogInformation($"{ClassFullName}: JobProfile Id: {messageContentId}: Segment previously updated");
                    break;

                case HttpStatusCode.Accepted:
                    log.LogWarning($"{ClassFullName}: JobProfile Id: {messageContentId}: Upserted segment, but Apprenticeship/Course refresh failed");
                    break;

                default:
                    log.LogWarning($"{ClassFullName}: JobProfile Id: {messageContentId}: Segment not Posted: Status: {result}");
                    break;
            }
        }
    }
}