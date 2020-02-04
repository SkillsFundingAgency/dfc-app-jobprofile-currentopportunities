using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Controllers
{
    public class CourseFeedController : Controller
    {
        private readonly ILogService logService;
        private readonly ICourseCurrentOpportunitiesRefresh courseCurrentOpportunitiesRefresh;

        public CourseFeedController(ILogService logService, ICourseCurrentOpportunitiesRefresh courseCurrentOpportunitiesRefresh)
        {
            this.logService = logService;
            this.courseCurrentOpportunitiesRefresh = courseCurrentOpportunitiesRefresh;
        }

        [HttpGet]
        [Route("CourseFeed/RefreshCourses/{documentId}")]
        public async Task<IActionResult> RefreshCourses(Guid documentId)
        {
            logService.LogInformation($"{nameof(RefreshCourses)} has been called with document Id {documentId}");
            var feedRefreshResponseViewModel = new FeedRefreshResponseViewModel();
            try
            {
                //catch any exception that the outgoing request may throw.
                feedRefreshResponseViewModel.NumberPulled = await courseCurrentOpportunitiesRefresh.RefreshCoursesAsync(documentId).ConfigureAwait(false);
                logService.LogInformation($"Get courses has succeeded for: document {documentId} - Got {feedRefreshResponseViewModel.NumberPulled} courses");
                return Ok(feedRefreshResponseViewModel);
            }
            catch (HttpRequestException httpRequestException)
            {
                feedRefreshResponseViewModel.RequestErrorMessage = httpRequestException.ToString();
                logService.LogError($"{nameof(RefreshCourses)} had exception when getting courses for document {documentId}, Exception - {feedRefreshResponseViewModel.RequestErrorMessage}");
                return BadRequest(feedRefreshResponseViewModel);
            }
        }
    }
}