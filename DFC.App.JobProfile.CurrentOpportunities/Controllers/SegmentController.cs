using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Extensions;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Controllers
{
    public class SegmentController : Controller
    {
        private readonly ILogger<SegmentController> logger;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly AutoMapper.IMapper mapper;

        public SegmentController(ILogger<SegmentController> logger, ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService, AutoMapper.IMapper mapper)
        {
            this.logger = logger;
            this.currentOpportunitiesSegmentService = currentOpportunitiesSegmentService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            logger.LogInformation($"{nameof(Index)} has been called");

            var viewModel = new IndexViewModel();
            var currentOpportunitiesSegmentModels = await currentOpportunitiesSegmentService.GetAllAsync().ConfigureAwait(false);

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

            var currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

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
        [Route("segment/simplelist")]
        public async Task<IActionResult> SimpleList()
        {
            logger.LogInformation($"{nameof(SimpleList)} has been called");

            var viewModel = new SimpleListViewModel();
            var currentOpportunitiesSegmentModels = await currentOpportunitiesSegmentService.GetAllAsync().ConfigureAwait(false);

            if (currentOpportunitiesSegmentModels != null)
            {
                viewModel.Items = (from a in currentOpportunitiesSegmentModels.OrderBy(o => o.CanonicalName)
                                   select mapper.Map<SimpleDocumentViewModel>(a)).ToList();

                logger.LogInformation($"{nameof(SimpleList)} has succeeded");
            }
            else
            {
                logger.LogWarning($"{nameof(SimpleList)} has returned with no results");
            }

            return this.NegotiateContentResult(viewModel, viewModel.Items);
        }

        [HttpGet]
        [Route("segment/{documentId}/contents")]
        public async Task<IActionResult> Body(Guid documentId)
        {
            logger.LogInformation($"{nameof(Body)} has been called with: {documentId}");
            var currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (currentOpportunitiesSegmentModel != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(currentOpportunitiesSegmentModel);
                logger.LogInformation($"{nameof(Body)} has succeeded for: {documentId}");
                return this.NegotiateContentResult(viewModel, currentOpportunitiesSegmentModel.Data);
            }

            logger.LogWarning($"{nameof(Body)} has returned no content for: {documentId}");

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

            var response = await currentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(CreateOrUpdate)} has upserted content for: {currentOpportunitiesSegmentModel.CanonicalName}");

            return new StatusCodeResult((int)response);
        }

        [HttpDelete]
        [Route("segment/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logger.LogInformation($"{nameof(Delete)} has been called");

            var currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (currentOpportunitiesSegmentModel == null)
            {
                logger.LogWarning($"{nameof(Document)} has returned no content for: {documentId}");

                return NotFound();
            }

            await currentOpportunitiesSegmentService.DeleteAsync(documentId).ConfigureAwait(false);

            logger.LogInformation($"{nameof(Delete)} has deleted content for: {currentOpportunitiesSegmentModel.CanonicalName}");

            return Ok();
        }
    }
}
