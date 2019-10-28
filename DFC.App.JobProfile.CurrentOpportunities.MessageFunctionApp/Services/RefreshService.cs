using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services
{
    public class RefreshService : IRefreshService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<RefreshService> logger;
        public readonly RefreshClientOptions refreshClientOptions;

        public RefreshService(
                    HttpClient httpClient,
                    ILogger<RefreshService> logger,
                    RefreshClientOptions refreshClientOptions)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.refreshClientOptions = refreshClientOptions;
        }

        public async Task<IList<SimpleJobProfileModel>> GetSimpleListAsync()
        {
            var url = $"{refreshClientOptions.BaseAddress}segment/simplelist";

            logger.LogInformation($"{nameof(GetSimpleListAsync)}: Loading simple list from {url}");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<List<SimpleJobProfileModel>>(responseString);

                logger.LogInformation($"{nameof(GetSimpleListAsync)}: Loaded simple list from {url}");

                return result;
            }

            logger.LogError($"{nameof(GetSimpleListAsync)}: Error Loading simple list from {url}, status: {response.StatusCode}");

            return null;
        }

        public async Task<HttpStatusCode> RefreshApprenticeshipsAsync(Guid documentId)
        {
            var url = $"{refreshClientOptions.BaseAddress}AVFeed/RefreshApprenticeships/{documentId}";
            HttpResponseMessage response = null;

            try
            {
                logger.LogInformation($"{nameof(RefreshApprenticeshipsAsync)}: Refreshing Job Profile Apprenticeships from {url}");

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

                response = await httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    logger.LogInformation($"{nameof(RefreshApprenticeshipsAsync)}: Refreshed Job Profile Apprenticeships from {url}");
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var result = JsonConvert.DeserializeObject<FeedRefreshResponsModel>(responseString);

                    logger.LogError($"{nameof(RefreshApprenticeshipsAsync)}: Error refreshing Job Profile Apprenticeships from {url}, status: {response.StatusCode}, response message: {result.RequestErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(RefreshApprenticeshipsAsync)}: Error refreshing Job Profile Apprenticeships from {url}, status: {response?.StatusCode}");
            }

            return response?.StatusCode ?? HttpStatusCode.InternalServerError;
        }

        public async Task<HttpStatusCode> RefreshCoursesAsync(Guid documentId)
        {
            var url = $"{refreshClientOptions.BaseAddress}CourseFeed/RefreshCourses/{documentId}";
            HttpResponseMessage response = null;

            try
            {
                logger.LogInformation($"{nameof(RefreshCoursesAsync)}: Refreshing Job Profile Courses from {url}");

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

                response = await httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    logger.LogInformation($"{nameof(RefreshCoursesAsync)}: Refreshed Job Profile Courses from {url}");
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var result = JsonConvert.DeserializeObject<FeedRefreshResponsModel>(responseString);

                    logger.LogError($"{nameof(RefreshCoursesAsync)}: Error refreshing Job Profile Courses from {url}, status: {response.StatusCode}, response message: {result.RequestErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(RefreshCoursesAsync)}: Error refreshing Job Profile Courses from {url}, status: {response?.StatusCode}");
            }

            return response?.StatusCode ?? HttpStatusCode.InternalServerError;
        }
    }
}
