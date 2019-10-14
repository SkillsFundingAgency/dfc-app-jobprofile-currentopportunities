using DFC.App.JobProfile.CurrentOpportunities.Data.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Extensions;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Controllers
{
    public class HealthController : Controller
    {
        private readonly ILogger<HealthController> logger;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly IAVAPIService aVAPIService;
        private readonly AutoMapper.IMapper mapper;

        public HealthController(ILogger<HealthController> logger, ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService, IAVAPIService aVAPIService, AutoMapper.IMapper mapper)
        {
            this.logger = logger;
            this.currentOpportunitiesSegmentService = currentOpportunitiesSegmentService;
            this.aVAPIService = aVAPIService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            logger.LogInformation($"{nameof(Health)} has been called");

            var viewModel = new HealthViewModel() { HealthItems = new List<HealthItemViewModel>() };

            viewModel.HealthItems.Add(mapper.Map<HealthItemViewModel>(await currentOpportunitiesSegmentService.GetCurrentHealthStatusAsync().ConfigureAwait(false)));
            viewModel.HealthItems.Add(mapper.Map<HealthItemViewModel>(await aVAPIService.GetCurrentHealthStatusAsync().ConfigureAwait(false)));

            foreach (var healthItem in viewModel.HealthItems)
            {
                LogResponse(healthItem);
            }

            //if we have any state thats is not green
            if (viewModel.HealthItems.Any(s => s.HealthServiceState != HealthServiceState.Green))
            {
                this.HttpContext.Response.StatusCode = 502;
            }

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("health/ping")]
        public IActionResult Ping()
        {
            logger.LogInformation($"{nameof(Ping)} has been called");

            return Ok();
        }

        private void LogResponse(HealthItemViewModel healthItemViewModel)
        {
            var message = $"{nameof(Health)} responded with: {healthItemViewModel.Service} - {healthItemViewModel.SubService} - {healthItemViewModel.HealthServiceState} - {healthItemViewModel.CheckParametersUsed} - {healthItemViewModel.Message}";
            if (healthItemViewModel.HealthServiceState == HealthServiceState.Red)
            {
                logger.LogError(message);
            }
            else
            {
                logger.LogInformation(message);
            }
        }
    }
}
