using DFC.App.JobProfile.CurrentOpportunities.Data.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Extensions;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Controllers
{
    public class SegmentController : Controller
    {
        private readonly ILogger<SegmentController> logger;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentModelegmentService;
        private readonly AutoMapper.IMapper mapper;
        private readonly CourseSearchConfig courseSearchConfig;

        public SegmentController(ILogger<SegmentController> logger, ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService, AutoMapper.IMapper mapper, CourseSearchConfig courseSearchConfig)
        {
            this.logger = logger;
            this.currentOpportunitiesSegmentModelegmentService = currentOpportunitiesSegmentService;
            this.mapper = mapper;
            this.courseSearchConfig = courseSearchConfig;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            logger.LogInformation($"{nameof(Index)} has been called");

            var viewModel = new IndexViewModel();
            var currentOpportunitiesSegmentModels = await currentOpportunitiesSegmentModelegmentService.GetAllAsync().ConfigureAwait(false);

            if (currentOpportunitiesSegmentModels != null)
            {
                viewModel.Documents = (from a in currentOpportunitiesSegmentModels.OrderBy(o => o.CanonicalName)
                                       select mapper.Map<IndexDocumentViewModel>(a)).ToList();

                logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("segment/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            logger.LogInformation($"{nameof(Document)} has been called with: {article}");

            var currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentModelegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (currentOpportunitiesSegmentModel != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(currentOpportunitiesSegmentModel);

                logger.LogInformation($"{nameof(Document)} has succeeded for: {article}");

                return View(viewModel);
            }

            logger.LogWarning($"{nameof(Document)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("segment/{article}/contents")]
        public async Task<IActionResult> Body(string article)
        {
            logger.LogInformation($"{nameof(Body)} has been called with: {article}");
            var currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentModelegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (currentOpportunitiesSegmentModel != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(currentOpportunitiesSegmentModel);
                viewModel.Data.CourseSearchUrl = courseSearchConfig.CourseSearchUrl;
                logger.LogInformation($"{nameof(Body)} has succeeded for: {article}");
                return this.NegotiateContentResult(viewModel);
            }

            logger.LogWarning($"{nameof(Body)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpPut]
        [HttpPost]
        [Route("segment")]
        public async Task<IActionResult> CreateOrUpdate([FromBody]CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel)
        {
            logger.LogInformation($"{nameof(CreateOrUpdate)} has been called");

            if (currentOpportunitiesSegmentModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCareerPathSegmentModel = await currentOpportunitiesSegmentModelegmentService.GetByIdAsync(currentOpportunitiesSegmentModel.DocumentId).ConfigureAwait(false);

            if (existingCareerPathSegmentModel == null)
            {
                var createdResponse = await currentOpportunitiesSegmentModelegmentService.CreateAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

                logger.LogInformation($"{nameof(CreateOrUpdate)} has created content for: {currentOpportunitiesSegmentModel.CanonicalName}");

                return new CreatedAtActionResult(nameof(Document), "Segment", new { article = createdResponse.CanonicalName }, createdResponse);
            }
            else
            {
                var updatedResponse = await currentOpportunitiesSegmentModelegmentService.ReplaceAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

                logger.LogInformation($"{nameof(CreateOrUpdate)} has updated content for: {currentOpportunitiesSegmentModel.CanonicalName}");

                return new OkObjectResult(updatedResponse);
            }
        }

        [HttpDelete]
        [Route("segment/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logger.LogInformation($"{nameof(Delete)} has been called");

            var currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentModelegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (currentOpportunitiesSegmentModel == null)
            {
                logger.LogWarning($"{nameof(Document)} has returned no content for: {documentId}");

                return NotFound();
            }

            await currentOpportunitiesSegmentModelegmentService.DeleteAsync(documentId, currentOpportunitiesSegmentModel.PartitionKey).ConfigureAwait(false);

            logger.LogInformation($"{nameof(Delete)} has deleted content for: {currentOpportunitiesSegmentModel.CanonicalName}");

            return Ok();
        }
    }
}