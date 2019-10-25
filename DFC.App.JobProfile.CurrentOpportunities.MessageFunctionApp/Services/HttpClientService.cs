using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.HttpClientPolicies;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services
{
    public static class HttpClientService
    {
        //public static async Task<CurrentOpportunitiesSegmentModel> GetByIdAsync(HttpClient httpClient, SegmentClientOptions segmentClientOptions, Guid id)
        //{
        //    var url = $"{segmentClientOptions.BaseAddress}segment/{id}/contents";

        //    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
        //    {
        //        request.Headers.Accept.Clear();
        //        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

        //        var response = await httpClient.SendAsync(request).ConfigureAwait(false);

        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        //            var result = JsonConvert.DeserializeObject<CurrentOpportunitiesSegmentModel>(responseString);

        //            return result;
        //        }
        //    }

        //    return default(CurrentOpportunitiesSegmentModel);
        //}

        //public static async Task<HttpStatusCode> PatchAsync(HttpClient httpClient, SegmentClientOptions segmentClientOptions, CurrentOpportunitiesPatchSegmentModel careerPathPatchSegmentModel, Guid documentId)
        //{
        //    var url = $"{segmentClientOptions.BaseAddress}segment/{documentId}/content-type/markup";

        //    using (var request = new HttpRequestMessage(HttpMethod.Patch, url))
        //    {
        //        request.Headers.Accept.Clear();
        //        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        //        request.Content = new ObjectContent(typeof(CurrentOpportunitiesPatchSegmentModel), careerPathPatchSegmentModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json);

        //        var response = await httpClient.SendAsync(request).ConfigureAwait(false);

        //        return response.StatusCode;
        //    }
        //}

        //public static async Task<HttpStatusCode> PostAsync(HttpClient httpClient, SegmentClientOptions segmentClientOptions, CurrentOpportunitiesSegmentModel careerPathSegmentModel)
        //{
        //    var url = $"{segmentClientOptions.BaseAddress}segment";

        //    using (var request = new HttpRequestMessage(HttpMethod.Post, url))
        //    {
        //        request.Headers.Accept.Clear();
        //        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        //        request.Content = new ObjectContent(typeof(CurrentOpportunitiesSegmentModel), careerPathSegmentModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json);

        //        var response = await httpClient.SendAsync(request).ConfigureAwait(false);

        //        return response.StatusCode;
        //    }
        //}

        //public static async Task<HttpStatusCode> DeleteAsync(HttpClient httpClient, SegmentClientOptions segmentClientOptions, Guid id)
        //{
        //    var url = $"{segmentClientOptions.BaseAddress}segment/{id}";

        //    using (var request = new HttpRequestMessage(HttpMethod.Delete, url))
        //    {
        //        request.Headers.Accept.Clear();
        //        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

        //        var response = await httpClient.SendAsync(request).ConfigureAwait(false);

        //        return response.StatusCode;
        //    }
        //}
    }
}
