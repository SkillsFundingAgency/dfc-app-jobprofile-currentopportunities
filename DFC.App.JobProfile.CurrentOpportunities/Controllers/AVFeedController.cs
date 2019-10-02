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
        private readonly IAVCurrentOpportunatiesUpdate aVCurrentOpportunatiesUpdate;


        public AVFeedController(ILogger<AVFeedController> logger, IAVCurrentOpportunatiesUpdate aVCurrentOpportunatiesUpdate)
        {
            this.logger = logger;
            this.aVCurrentOpportunatiesUpdate = aVCurrentOpportunatiesUpdate;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await aVCurrentOpportunatiesUpdate.UpdateApprenticeshipVacanciesAsync("jobprofile1").ConfigureAwait(false);
            return Ok();
        }
    }
}