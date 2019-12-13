using DFC.App.JobProfile.CurrentOpportunities.Data;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels;
using DFC.App.JobProfile.CurrentOpportunities.Extensions;
using DFC.App.JobProfile.CurrentOpportunities.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Controllers
{
    public class SegmentController : Controller
    {
        private readonly ILogService logService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly AutoMapper.IMapper mapper;

        public SegmentController(ILogService logService, ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService, AutoMapper.IMapper mapper)
        {
            this.logService = logService;
            this.currentOpportunitiesSegmentService = currentOpportunitiesSegmentService;
            this.mapper = mapper;
        }

        private static string GetJobTitleWithPrefix(string titlePrefix, string title, string contentTitle)
        {
            var changedTitle = string.IsNullOrWhiteSpace(contentTitle) ? title?.ToLower(new CultureInfo("en-GB")) : contentTitle;

            if (string.IsNullOrWhiteSpace(changedTitle))
            {
                return string.Empty;
            }

            switch (titlePrefix)
            {
                case Constants.TitleNoPrefix:
                    return $"{changedTitle}";

                case Constants.TitlePrefixWithA:
                    return $"a {changedTitle}";

                case Constants.TitlePrefixWithAn:
                    return $"an {changedTitle}";

                case Constants.TitleNoTitle:
                    return string.Empty;

                default:
                    return GetDefaultDynamicTitle(changedTitle);
            }
        }

        private static string GetDefaultDynamicTitle(string title) => IsStartsWithVowel(title) ? $"an {title}" : $"a {title}";

        private static bool IsStartsWithVowel(string title) => new[] { 'a', 'e', 'i', 'o', 'u' }.Contains(title.ToLower(new CultureInfo("en-GB")).First());

        [HttpGet]
        [Route("/")]
        [Route("/Segment")]
        public async Task<IActionResult> Index()
        {
            logService.LogInformation($"{nameof(Index)} has been called");

            var viewModel = new IndexViewModel();
            var currentOpportunitiesSegmentModels = await currentOpportunitiesSegmentService.GetAllAsync().ConfigureAwait(false);

            if (currentOpportunitiesSegmentModels != null)
            {
                viewModel.Documents = (from a in currentOpportunitiesSegmentModels.OrderBy(o => o.CanonicalName)
                                       select mapper.Map<IndexDocumentViewModel>(a)).ToList();

                logService.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                logService.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return this.NegotiateContentResult(viewModel, viewModel.Documents);
        }

        [HttpGet]
        [Route("segment/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            logService.LogInformation($"{nameof(Document)} has been called with: {article}");

            var currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentService.GetByNameAsync(article).ConfigureAwait(false);

            if (currentOpportunitiesSegmentModel != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(currentOpportunitiesSegmentModel);

                logService.LogInformation($"{nameof(Document)} has succeeded for: {article}");

                return View(viewModel);
            }

            logService.LogWarning($"{nameof(Document)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("segment/{documentId}/contents")]
        public async Task<IActionResult> Body(Guid documentId)
        {
            logService.LogInformation($"{nameof(Body)} has been called with: {documentId}");
            var currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (currentOpportunitiesSegmentModel != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(currentOpportunitiesSegmentModel);
                viewModel.Data.JobTitleWithPrefix = GetJobTitleWithPrefix(currentOpportunitiesSegmentModel.Data?.TitlePrefix, currentOpportunitiesSegmentModel.Data?.JobTitle, currentOpportunitiesSegmentModel.Data?.ContentTitle);
                logService.LogInformation($"{nameof(Body)} has succeeded for: {documentId}");
                return this.NegotiateContentResult(viewModel, currentOpportunitiesSegmentModel.Data);
            }

            logService.LogWarning($"{nameof(Body)} has returned no content for: {documentId}");

            return NoContent();
        }

        [HttpPost]
        [Route("segment")]
        public async Task<IActionResult> Post([FromBody]CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel)
        {
            logService.LogInformation($"{nameof(Post)} has been called");

            if (currentOpportunitiesSegmentModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDocument = await currentOpportunitiesSegmentService.GetByIdAsync(currentOpportunitiesSegmentModel.DocumentId).ConfigureAwait(false);
            if (existingDocument != null)
            {
                return new StatusCodeResult((int)HttpStatusCode.AlreadyReported);
            }

            var response = await currentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            logService.LogInformation($"{nameof(Post)} has upserted content for: {currentOpportunitiesSegmentModel.CanonicalName}");

            return new StatusCodeResult((int)response);
        }

        [HttpPut]
        [Route("segment")]
        public async Task<IActionResult> Put([FromBody]CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel)
        {
            logService.LogInformation($"{nameof(Put)} has been called");

            if (currentOpportunitiesSegmentModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDocument = await currentOpportunitiesSegmentService.GetByIdAsync(currentOpportunitiesSegmentModel.DocumentId).ConfigureAwait(false);
            if (existingDocument == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            if (currentOpportunitiesSegmentModel.SequenceNumber <= existingDocument.SequenceNumber)
            {
                return new StatusCodeResult((int)HttpStatusCode.AlreadyReported);
            }

            currentOpportunitiesSegmentModel.Etag = existingDocument.Etag;
            currentOpportunitiesSegmentModel.SocLevelTwo = existingDocument.SocLevelTwo;

            var response = await currentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);

            logService.LogInformation($"{nameof(Put)} has upserted content for: {currentOpportunitiesSegmentModel.CanonicalName}");

            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("segment/{documentId}/JobProfileSoc")]
        public async Task<IActionResult> PatchJobProfileSoc([FromBody]PatchJobProfileSocModel patchJobProfileSocModel, Guid documentId)
        {
            logService.LogInformation($"{nameof(PatchJobProfileSoc)} has been called");

            if (patchJobProfileSocModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await currentOpportunitiesSegmentService.PatchJobProfileSocAsync(patchJobProfileSocModel, documentId).ConfigureAwait(false);
            if (response != HttpStatusCode.OK && response != HttpStatusCode.Created)
            {
                logService.LogError($"{nameof(PatchJobProfileSoc)}: Error while patching Soc Data content for Job Profile with Id: {patchJobProfileSocModel.JobProfileId} for the {patchJobProfileSocModel.SocCode} soc code");
            }

            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("segment/{documentId}/ApprenticeshipFrameworks")]
        public async Task<IActionResult> PatchApprenticeshipFrameworks([FromBody]PatchApprenticeshipFrameworksModel patchApprenticeshipFrameworksModel, Guid documentId)
        {
            logService.LogInformation($"{nameof(PatchApprenticeshipFrameworks)} has been called");

            if (patchApprenticeshipFrameworksModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await currentOpportunitiesSegmentService.PatchApprenticeshipFrameworksAsync(patchApprenticeshipFrameworksModel, documentId).ConfigureAwait(false);
            if (response != HttpStatusCode.OK && response != HttpStatusCode.Created)
            {
                logService.LogError($"{nameof(PatchApprenticeshipFrameworks)}: Error while patching Apprenticeship Frameworks content for Job Profile with Id: {patchApprenticeshipFrameworksModel.JobProfileId} for the {patchApprenticeshipFrameworksModel.SocCode} soc code");
            }

            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("segment/{documentId}/ApprenticeshipStandards")]
        public async Task<IActionResult> PatchApprenticeshipStandards([FromBody]PatchApprenticeshipStandardsModel patchApprenticeshipStandardsModel, Guid documentId)
        {
            logService.LogInformation($"{nameof(PatchApprenticeshipStandards)} has been called");

            if (patchApprenticeshipStandardsModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await currentOpportunitiesSegmentService.PatchApprenticeshipStandardsAsync(patchApprenticeshipStandardsModel, documentId).ConfigureAwait(false);
            if (response != HttpStatusCode.OK && response != HttpStatusCode.Created)
            {
                logService.LogError($"{nameof(PatchApprenticeshipStandards)}: Error while patching Apprenticeship Standards content for Job Profile with Id: {patchApprenticeshipStandardsModel.JobProfileId} for the {patchApprenticeshipStandardsModel.SocCode} soc code");
            }

            return new StatusCodeResult((int)response);
        }

        [HttpDelete]
        [Route("segment/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logService.LogInformation($"{nameof(Delete)} has been called");

            var currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (currentOpportunitiesSegmentModel == null)
            {
                logService.LogWarning($"{nameof(Document)} has returned no content for: {documentId}");

                return NotFound();
            }

            await currentOpportunitiesSegmentService.DeleteAsync(documentId).ConfigureAwait(false);

            logService.LogInformation($"{nameof(Delete)} has deleted content for: {currentOpportunitiesSegmentModel.CanonicalName}");

            return Ok();
        }
    }
}
