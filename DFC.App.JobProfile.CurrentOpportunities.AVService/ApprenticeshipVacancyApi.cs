using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService
{ 
    public class ApprenticeshipVacancyApi : IApprenticeshipVacancyApi
    {
        private readonly ILogger<ApprenticeshipVacancyApi> logger;
        private readonly IAuditService auditService;
        private readonly AVAPIServiceSettings aVAPIServiceSettings;

        public ApprenticeshipVacancyApi(ILogger<ApprenticeshipVacancyApi> logger, IAuditService auditService, AVAPIServiceSettings aVAPIServiceSettings)
        {
            this.logger = logger;
            this.auditService = auditService;
            this.aVAPIServiceSettings = aVAPIServiceSettings;
        }

        public async Task<string> GetAsync(string requestQueryString, RequestType requestType )
        {
            using (var clientProxy = new HttpClient())
            {
                clientProxy.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", aVAPIServiceSettings.FAASubscriptionKey);
                clientProxy.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

                var requestRoute = requestType == RequestType.search ? $"{requestType.ToString()}?" : string.Empty;
                var fullRequest = $"{aVAPIServiceSettings.FAAEndPoint}/{requestRoute}{requestQueryString}";
                logger.LogInformation($"Getting API data for request :'{fullRequest}'");

                var response = await clientProxy.GetAsync(fullRequest).ConfigureAwait(false);
                string responseContent = await response.Content?.ReadAsStringAsync();
                await auditService.AuditAsync(responseContent, requestQueryString);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"Error status {response.StatusCode},  Getting API data for request :'{fullRequest}' \nResponse : {responseContent}", null );

                    //this will throw an exception as is not a success code
                    response.EnsureSuccessStatusCode();
                }
                return responseContent;
            }
        }
    }
}
