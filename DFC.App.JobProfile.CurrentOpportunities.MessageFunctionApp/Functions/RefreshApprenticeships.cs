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
    public static class RefreshApprenticeships
    {
        [FunctionName("RefreshApprenticeships")]
        public static async System.Threading.Tasks.Task RunAsync(
            [TimerTrigger("%RefreshApprenticeshipsCron%")]TimerInfo myTimer,
            ILogger log,
            [Inject] IRefreshService refreshService)
        {
            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function starting at: {DateTime.Now}, using TimerInfo: {myTimer.Schedule.ToString()}");

            int abortAfterErrorCount = 10;
            int errorCount = 0;
            int totalErrorCount = 0;
            int totalSuccessCount = 0;

            _ = int.TryParse(Environment.GetEnvironmentVariable(nameof(abortAfterErrorCount)), out abortAfterErrorCount);

            HttpStatusCode statusCode = HttpStatusCode.OK;

            var simpleJobProfileModels = await refreshService.GetListAsync().ConfigureAwait(false);

            if (simpleJobProfileModels != null)
            {
                log.LogInformation($"{nameof(RefreshApprenticeships)}: Retrieved {simpleJobProfileModels.Count} Job Profiles");

                foreach (var simpleJobProfileModel in simpleJobProfileModels)
                {
                    log.LogInformation($"{nameof(RefreshApprenticeships)}: Refreshing Job Profile Apprenticeships: {simpleJobProfileModel.DocumentId} / {simpleJobProfileModel.CanonicalName}");

                    statusCode = await refreshService.RefreshApprenticeshipsAsync(simpleJobProfileModel.DocumentId).ConfigureAwait(false);

                    switch (statusCode)
                    {
                        case HttpStatusCode.OK:
                            errorCount = 0;
                            totalSuccessCount++;
                            log.LogInformation($"{nameof(RefreshApprenticeships)}: Refreshed Job Profile Apprenticeships: {simpleJobProfileModel.DocumentId} / {simpleJobProfileModel.CanonicalName}");
                            break;

                        default:
                            errorCount++;
                            totalErrorCount++;
                            log.LogError($"{nameof(RefreshApprenticeships)}: Error refreshing Job Profile Apprenticeships: {simpleJobProfileModel.DocumentId} / {simpleJobProfileModel.CanonicalName} - Status code = {statusCode}");
                            break;
                    }

                    if (errorCount >= abortAfterErrorCount)
                    {
                        log.LogWarning($"{nameof(RefreshApprenticeships)}: Timer trigger aborting after {abortAfterErrorCount} consecutive errors");
                        break;
                    }
                }
            }

            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function, Apprenticeships refreshed: {totalSuccessCount}");
            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function, Apprenticeships refresh errors: {totalErrorCount}");
            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function completed at: {DateTime.Now}");

            // if we aborted due to the number of errors exceeding the abortAfterErrorCount
            if (errorCount >= abortAfterErrorCount)
            {
                throw new HttpResponseException(new HttpResponseMessage() { StatusCode = statusCode, ReasonPhrase = $"Timer trigger aborting after {abortAfterErrorCount} consecutive errors" });
            }
        }
    }
}
