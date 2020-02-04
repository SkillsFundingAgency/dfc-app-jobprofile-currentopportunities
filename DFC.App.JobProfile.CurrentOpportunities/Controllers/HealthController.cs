using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Extensions;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Controllers
{
    public class HealthController : Controller
    {
        private const string SuccessMessage = "Document store is available";

        private readonly ILogService logService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly string resourceName;

        public HealthController(ILogService logService, ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService)
        {
            this.logService = logService;
            this.currentOpportunitiesSegmentService = currentOpportunitiesSegmentService;
            resourceName = typeof(Program).Namespace;
        }

        [HttpGet]
        public IActionResult Ping()
        {
            logService.LogInformation($"{nameof(Ping)} has been called");

            return Ok();
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            logService.LogInformation($"{nameof(Health)} has been called");

            try
            {
                var isHealthy = await currentOpportunitiesSegmentService.PingAsync().ConfigureAwait(false);
                if (isHealthy)
                {
                    logService.LogInformation($"{nameof(Health)} responded with: {resourceName} - {SuccessMessage}");

                    var viewModel = CreateHealthViewModel();

                    return this.NegotiateContentResult(viewModel);
                }

                logService.LogError($"{nameof(Health)}: Ping to {resourceName} has failed");
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(Health)}: {resourceName} exception: {ex.Message}");
            }

            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        private HealthViewModel CreateHealthViewModel()
        {
            return new HealthViewModel
            {
                HealthItems = new List<HealthItemViewModel>
                {
                    new HealthItemViewModel
                    {
                        Service = resourceName,
                        Message = SuccessMessage,
                    },
                },
            };
        }
    }
}
