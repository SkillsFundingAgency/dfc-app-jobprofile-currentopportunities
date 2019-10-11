using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Functions
{

    public static class PostSegment
    {
        private static readonly string ThisClassPath = typeof(PostSegment).FullName;

        [FunctionName("PostSegment")]
        public static async Task Run(
                                        [ServiceBusTrigger("%post-segment-topic-name%", "%post-segment-subscription-name%", Connection = "service-bus-connection-string")] string serviceBusMessage,
                                        ILogger log,
                                        [Inject] SegmentClientOptions segmentClientOptions,
                                        [Inject] HttpClient httpClient)
        {
            var serviceBusModel = JsonConvert.DeserializeObject<CurrentOpportunitiesSegmentModel>(serviceBusMessage);

            log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.DocumentId}: Posting segment");

            var segmentModel = await HttpClientService.GetByIdAsync(httpClient, segmentClientOptions, serviceBusModel.DocumentId).ConfigureAwait(false);

            if (segmentModel == null || segmentModel.Data.LastReviewed < serviceBusModel.Data?.LastReviewed)
            {
                var result = await HttpClientService.PostAsync(httpClient, segmentClientOptions, serviceBusModel).ConfigureAwait(false);

                if (result == HttpStatusCode.OK)
                {
                    log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.DocumentId}: Updated segment");
                }
                else if (result == HttpStatusCode.Created)
                {
                    log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.DocumentId}: Created segment");
                }
                else
                {
                    log.LogWarning($"{ThisClassPath}: JobProfile Id: {serviceBusModel.DocumentId}: Segment not Posted: Status: {result}");
                }
            }
            else
            {
                log.LogWarning($"{ThisClassPath}: JobProfile Id: {serviceBusModel.DocumentId}: Service bus message is stale: {serviceBusModel.Data.LastReviewed}, stored: {segmentModel.Data.LastReviewed}");
            }
        }
    }
}
