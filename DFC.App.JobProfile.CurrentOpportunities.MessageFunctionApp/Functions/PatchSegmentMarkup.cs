using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.ServiceBusModels;
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
    public static class PatchSegmentMarkup
    {
        private static readonly string ThisClassPath = typeof(PatchSegmentMarkup).FullName;

        //[FunctionName("PatchSegmentMarkup")]
        //public static async Task Run(
        //                                [ServiceBusTrigger("%patch-segment-topic-name%", "%patch-segment-subscription-name%", Connection = "service-bus-connection-string")] string serviceBusMessage,
        //                                ILogger log,
        //                                [Inject] SegmentClientOptions segmentClientOptions)
        //{
        //    var serviceBusModel = JsonConvert.DeserializeObject<CurrentOpportunitiesPatchMarkupServiceBusModel>(serviceBusMessage);

        //    log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Patching segment");

        //    var httpClient = new HttpClient();
        //    var segmentModel = await HttpClientService.GetByIdAsync(httpClient, segmentClientOptions, serviceBusModel.JobProfileId).ConfigureAwait(false);

        //    if (segmentModel == null || segmentModel.Data == null || segmentModel.Data.LastReviewed < serviceBusModel.Data.LastReviewed)
        //    {
        //        var careerPathPatchSegmentModel = new CurrentOpportunitiesPatchSegmentModel
        //        {
        //            SocLevelTwo = serviceBusModel.SocLevelTwo,
        //            CanonicalName = serviceBusModel.CanonicalName,
        //            Data = serviceBusModel.Data,
        //        };

        //        var result = await HttpClientService.PatchAsync(httpClient, segmentClientOptions, careerPathPatchSegmentModel, serviceBusModel.JobProfileId).ConfigureAwait(false);

        //        if (result == HttpStatusCode.OK)
        //        {
        //            log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Patched segment");
        //        }
        //        else
        //        {
        //            log.LogWarning($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Segment not patched: Status: {result}");
        //        }
        //    }
        //    else
        //    {
        //        log.LogWarning($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Service bus message is stale: {serviceBusModel.Data.LastReviewed}, stored: {segmentModel.Data.LastReviewed}");
        //    }
        //}
    }
}
