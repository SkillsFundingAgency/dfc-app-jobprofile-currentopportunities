using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Functions
{
    public static class RefreshCourses
    {
        [FunctionName("RefreshCourses")]
        public static async System.Threading.Tasks.Task RunAsync(
            [TimerTrigger("%RefreshCoursesCron%")]TimerInfo myTimer,
            [Inject] ILogger log,
            [Inject] IRefreshService refreshService)
        {
            log.LogInformation($"{nameof(RefreshCourses)}: Timer trigger function starting at: {DateTime.Now}, using TimerInfo: {myTimer.Schedule.ToString()}");

            var abortAfterErrorCount = 10;
            var errorCount = 0;
            var totalErrorCount = 0;
            var totalSuccessCount = 0;

            _ = int.TryParse(Environment.GetEnvironmentVariable(nameof(abortAfterErrorCount)), out abortAfterErrorCount);

            HttpStatusCode statusCode = HttpStatusCode.OK;

            var simpleJobProfileModels = await refreshService.GetListAsync().ConfigureAwait(false);

            if (simpleJobProfileModels != null)
            {
                log.LogInformation($"{nameof(RefreshCourses)}: Retrieved {simpleJobProfileModels.Count} Job Profiles");

                foreach (var simpleJobProfileModel in simpleJobProfileModels)
                {
                    log.LogInformation($"{nameof(RefreshCourses)}: Refreshing Job Profile Courses: {simpleJobProfileModel.DocumentId} / {simpleJobProfileModel.CanonicalName}");

                    statusCode = await refreshService.RefreshCoursesAsync(simpleJobProfileModel.DocumentId).ConfigureAwait(false);

                    switch (statusCode)
                    {
                        case HttpStatusCode.OK:
                            errorCount = 0;
                            totalSuccessCount++;
                            log.LogInformation($"{nameof(RefreshCourses)}: Refreshed Job Profile Courses: {simpleJobProfileModel.DocumentId} / {simpleJobProfileModel.CanonicalName}");
                            break;

                        default:
                            errorCount++;
                            totalErrorCount++;
                            log.LogError($"{nameof(RefreshCourses)}: Error refreshing Job Profile Courses: {simpleJobProfileModel.DocumentId} / {simpleJobProfileModel.CanonicalName} - Status code = {statusCode}");
                            break;
                    }

                    if (errorCount >= abortAfterErrorCount)
                    {
                        log.LogWarning($"{nameof(RefreshCourses)}: Timer trigger aborting after {abortAfterErrorCount} consecutive errors");
                        break;
                    }
                }
            }

            log.LogInformation($"{nameof(RefreshCourses)}: Timer trigger function, Courses refreshed: {totalSuccessCount}");
            log.LogInformation($"{nameof(RefreshCourses)}: Timer trigger function, Courses refresh errors: {totalErrorCount}");
            log.LogInformation($"{nameof(RefreshCourses)}: Timer trigger function completed at: {DateTime.Now}");

            // if we aborted due to the number of errors exceeding the abortAfterErrorCount
            if (errorCount >= abortAfterErrorCount)
            {
                throw new HttpResponseException(new HttpResponseMessage() { StatusCode = statusCode, ReasonPhrase = $"Timer trigger aborting after {abortAfterErrorCount} consecutive errors" });
            }
        }
    }
}
