using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        public async Task<IActionResult> Index()
        {
            await aVCurrentOpportunatiesRefresh.RefreshApprenticeshipVacanciesAsync("jobprofile1").ConfigureAwait(false);
            return Ok();
        }
    }
}