using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Controllers
{
    public class FeedsController : Controller
    {
        private readonly ILogger<FeedsController> logger;
        private readonly IAVCurrentOpportuntiesRefresh aVCurrentOpportunatiesRefresh;

        public FeedsController(ILogger<FeedsController> logger, IAVCurrentOpportuntiesRefresh aVCurrentOpportunatiesRefresh)
        {
            this.logger = logger;
            this.aVCurrentOpportunatiesRefresh = aVCurrentOpportunatiesRefresh;
        }

        [HttpGet]
        [Route("AVFeed/RefreshApprenticeships/{documentId}")]
        public async Task<IActionResult> RefreshApprenticeships(Guid documentId)
        {
            var feedRefreshResponseModel = new FeedRefreshResponseModel();
            try
            {
                //catch any exception that the outgoing request may throw.
                feedRefreshResponseModel.NumberPulled = await aVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(documentId).ConfigureAwait(false);
                return Ok(feedRefreshResponseModel);
            }
            catch (HttpRequestException httpRequestException)
            {
                feedRefreshResponseModel.RequestErrorMessage = httpRequestException.Message;
                return BadRequest(feedRefreshResponseModel);
            }
        }
    }
}