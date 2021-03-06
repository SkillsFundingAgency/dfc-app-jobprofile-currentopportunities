﻿using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Controllers
{
    public class FeedsController : Controller
    {
        private readonly ILogService logService;
        private readonly IAVCurrentOpportunitiesRefresh aVCurrentOpportunatiesRefresh;

        public FeedsController(ILogService logService, IAVCurrentOpportunitiesRefresh aVCurrentOpportunatiesRefresh)
        {
            this.logService = logService;
            this.aVCurrentOpportunatiesRefresh = aVCurrentOpportunatiesRefresh;
        }

        [HttpGet]
        [Route("AVFeed/RefreshApprenticeships/{documentId}")]
        public async Task<IActionResult> RefreshApprenticeships(Guid documentId)
        {
            logService.LogInformation($"{nameof(RefreshApprenticeships)} has been called with document Id {documentId}");

            var feedRefreshResponseViewModel = new FeedRefreshResponseViewModel();
            try
            {
                //catch any exception that the outgoing request may throw.
                feedRefreshResponseViewModel.NumberPulled = await aVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAndUpdateJobProfileAsync(documentId).ConfigureAwait(false);
                return Ok(feedRefreshResponseViewModel);
            }
            catch (HttpRequestException httpRequestException)
            {
                feedRefreshResponseViewModel.RequestErrorMessage = httpRequestException.Message;
                return BadRequest(feedRefreshResponseViewModel);
            }
        }
    }
}