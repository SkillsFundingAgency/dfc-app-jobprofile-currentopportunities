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
            var currentOpportunitiesSegmentModelSegmentModels = await currentOpportunitiesSegmentModelegmentService.GetAllAsync().ConfigureAwait(false);

            if (currentOpportunitiesSegmentModelSegmentModels != null)
            {
                viewModel.Documents = (from a in currentOpportunitiesSegmentModelSegmentModels.OrderBy(o => o.CanonicalName)
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

            var currentOpportunitiesSegmentModelSegmentModel = await currentOpportunitiesSegmentModelegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (currentOpportunitiesSegmentModelSegmentModel != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(currentOpportunitiesSegmentModelSegmentModel);

                logger.LogInformation($"{nameof(Document)} has succeeded for: {article}");

                return View(viewModel);
            }

            logger.LogWarning($"{nameof(Document)} has returned no content for: {article}");

            return NoContent();
        }
    }
}