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
    public static class DeleteSegment
    {
        private static readonly string ThisClassPath = typeof(DeleteSegment).FullName;

        //[FunctionName("DeleteSegment")]
        //public static async Task Run(
        //                                [ServiceBusTrigger("%delete-topic-name%", "%delete-subscription-name%", Connection = "service-bus-connection-string")] string serviceBusMessage,
        //                                ILogger log,
        //                                [Inject] SegmentClientOptions segmentClientOptions)
        //{
        //    var serviceBusModel = JsonConvert.DeserializeObject<CurrentOpportunitiesDeleteServiceBusModel>(serviceBusMessage);

        //    log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Deleting segment");

        //    var httpClient = new HttpClient();
        //    var segmentDataModel = await HttpClientService.GetByIdAsync(httpClient, segmentClientOptions, serviceBusModel.JobProfileId).ConfigureAwait(false);

        //    if (segmentDataModel == null || segmentDataModel.Data.LastReviewed < serviceBusModel.LastReviewed)
        //    {
        //        var result = await HttpClientService.DeleteAsync(httpClient, segmentClientOptions, serviceBusModel.JobProfileId).ConfigureAwait(false);

        //        if (result == HttpStatusCode.OK)
        //        {
        //            log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Deleted segment");
        //        }
        //        else
        //        {
        //            log.LogWarning($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Segment not deleted: Status: {result}");
        //        }
        //    }
        //}
    }
}
