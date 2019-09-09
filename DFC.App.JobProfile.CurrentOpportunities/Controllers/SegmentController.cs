using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Extensions;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Controllers
{
    public class SegmentController : Controller
    {
        private readonly ILogger<SegmentController> logger;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentModelegmentService;
        private readonly AutoMapper.IMapper mapper;

        public SegmentController(ILogger<SegmentController> logger, ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService, AutoMapper.IMapper mapper)
        {
            this.logger = logger;
            this.currentOpportunitiesSegmentModelegmentService = currentOpportunitiesSegmentService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("segment")]
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
                logger.LogInformation($"{nameof(Body)} has succeeded for: {article}");
                return this.NegotiateContentResult(viewModel);
            }

            logger.LogWarning($"{nameof(Body)} has returned no content for: {article}");

            return NoContent();
        }
    }
}