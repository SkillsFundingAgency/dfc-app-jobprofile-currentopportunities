using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService
{
    public class ApprenticeshipVacancyApi : IApprenticeshipVacancyApi
    {
        private readonly ILogger<ApprenticeshipVacancyApi> logger;
        private readonly ICosmosRepository<APIAuditRecord> auditRepository;
        private readonly AVAPIServiceSettings aVAPIServiceSettings;
        private readonly HttpClient httpClient;
        private readonly Guid correlationId;

        public ApprenticeshipVacancyApi(ILogger<ApprenticeshipVacancyApi> logger, ICosmosRepository<APIAuditRecord> auditRepository,  AVAPIServiceSettings aVAPIServiceSettings, HttpClient httpClient)
        {
            this.logger = logger;
            this.auditRepository = auditRepository;
            this.aVAPIServiceSettings = aVAPIServiceSettings ?? throw new ArgumentNullException(nameof(aVAPIServiceSettings));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            this.httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", aVAPIServiceSettings.FAASubscriptionKey);
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.Timeout = TimeSpan.FromSeconds(aVAPIServiceSettings.RequestTimeOutSeconds);
            correlationId = Guid.NewGuid();
        }

        public async Task<string> GetAsync(string requestQueryString, RequestType requestType)
        {
                var requestRoute = requestType == RequestType.Search ? $"{requestType.ToString()}?" : string.Empty;
                var fullRequest = $"{aVAPIServiceSettings.FAAEndPoint}/{requestRoute}{requestQueryString}";
                logger.LogInformation($"Getting API data for request :'{fullRequest}'");

                var response = await httpClient.GetAsync(new Uri(fullRequest)).ConfigureAwait(false);

                //Even if there is a bad response code still read and write the resposne into the audit as it may have information about the cause.
                string responseContent = await (response.Content?.ReadAsStringAsync()).ConfigureAwait(false);
                var auditRecord = new APIAuditRecord() { DocumentId = Guid.NewGuid(),  CorrelationId = correlationId, Request = fullRequest, Response = responseContent };
                await auditRepository.UpsertAsync(auditRecord).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"Error status {response.StatusCode},  Getting API data for request :'{fullRequest}' \nResponse : {responseContent}");

                    //this will throw an exception as is not a success code
                    response.EnsureSuccessStatusCode();
                }

                return responseContent;
            }
    }
}
