using DFC.App.FindACourseClient.Contracts;
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
    public class CourseFeedController : Controller
    {
        private readonly ILogger<FeedsController> logger;
        private readonly ICourseCurrentOpportuntiesRefresh courseCurrentOpportuntiesRefresh;

        public CourseFeedController(ILogger<FeedsController> logger, ICourseCurrentOpportuntiesRefresh courseCurrentOpportuntiesRefresh)
        {
            this.logger = logger;
            this.courseCurrentOpportuntiesRefresh = courseCurrentOpportuntiesRefresh;
        }

        [HttpGet]
        [Route("CourseFeed/RefreshCourses/{documentId}")]
        public async Task<IActionResult> RefreshCourses(Guid documentId)
        {
            logger.LogInformation($"{nameof(RefreshCourses)} has been called with document Id {documentId}");
            var feedRefreshResponseModel = new FeedRefreshResponseModel();
            try
            {
                //catch any exception that the outgoing request may throw.
                feedRefreshResponseModel.NumberPulled = await courseCurrentOpportuntiesRefresh.RefreshCoursesAsync(documentId).ConfigureAwait(false);
                logger.LogInformation($"Get courses has succeeded for: document {documentId} - Got {feedRefreshResponseModel.NumberPulled} courses");
                return Ok(feedRefreshResponseModel);
            }
            catch (HttpRequestException httpRequestException)
            {
                feedRefreshResponseModel.RequestErrorMessage = httpRequestException.Message;
                logger.LogError($"{nameof(RefreshCourses)} had exception when getting courses for document {documentId}, Exception - {feedRefreshResponseModel.RequestErrorMessage}");
                return BadRequest(feedRefreshResponseModel);
            }
        }
    }
}