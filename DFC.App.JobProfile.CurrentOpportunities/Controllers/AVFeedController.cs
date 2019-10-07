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
    public class AVFeedController : Controller
    {
        private readonly ILogger<AVFeedController> logger;
        private readonly IAVCurrentOpportuntiesRefresh aVCurrentOpportunatiesRefresh;


        public AVFeedController(ILogger<AVFeedController> logger, IAVCurrentOpportuntiesRefresh aVCurrentOpportunatiesRefresh)
        {
            this.logger = logger;
            this.aVCurrentOpportunatiesRefresh = aVCurrentOpportunatiesRefresh;
        }

        [HttpGet]
        [Route("AVFeed/RefreshApprenticeships/{documentId}")]
        public async Task<IActionResult> RefreshApprenticeships(Guid documentId)
        {
            var aVFeedRefreshResponseModel = new AVFeedRefreshResponseModel();
            try
            {
                //catch any exception that the outgoing request may throw.
                aVFeedRefreshResponseModel.NumberPulled = await aVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync(documentId).ConfigureAwait(false);
                return Ok(aVFeedRefreshResponseModel);
            }
            catch (HttpRequestException httpRequestException)
            {
                aVFeedRefreshResponseModel.RequestErrorMessage = httpRequestException.Message;
                return BadRequest(aVFeedRefreshResponseModel);
            }
        }
    }
}