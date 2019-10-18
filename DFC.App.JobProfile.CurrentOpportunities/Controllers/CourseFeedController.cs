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
        private readonly AutoMapper.IMapper mapper;

        public CourseFeedController(ILogger<FeedsController> logger, ICourseCurrentOpportuntiesRefresh courseCurrentOpportuntiesRefresh, AutoMapper.IMapper mapper)
        {
            this.logger = logger;
            this.courseCurrentOpportuntiesRefresh = courseCurrentOpportuntiesRefresh;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("CourseFeed/RefreshCourses/{documentId}")]
        public async Task<IActionResult> RefreshCourses(Guid documentId)
        {
            logger.LogInformation($"{nameof(RefreshCourses)} has been called with document Id {documentId}");
            var feedRefreshResponseViewModel = new FeedRefreshResponseViewModel();
            try
            {
                //catch any exception that the outgoing request may throw.
                var feedRefreshResponseModel = await courseCurrentOpportuntiesRefresh.RefreshCoursesAsync(documentId).ConfigureAwait(false);
                feedRefreshResponseViewModel = mapper.Map<FeedRefreshResponseViewModel>(feedRefreshResponseModel);

                logger.LogInformation($"Get courses has succeeded for: document {documentId} - Got {feedRefreshResponseViewModel.NumberPulled} courses");
                return Ok(feedRefreshResponseViewModel);
            }
            catch (HttpRequestException httpRequestException)
            {
                feedRefreshResponseViewModel.RequestErrorMessage = httpRequestException.ToString();
                logger.LogError($"{nameof(RefreshCourses)} had exception when getting courses for document {documentId}, Exception - {feedRefreshResponseViewModel.RequestErrorMessage}");
                return BadRequest(feedRefreshResponseViewModel);
            }
        }
    }
}