using DFC.App.FindACourseClient.Models.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using DFC.App.JobProfile.CurrentOpportunities.SegmentService;
using DFC.FindACourseClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.CourseService
{
    public class CourseCurrentOpportunitiesRefresh : ICourseCurrentOpportunitiesRefresh, IHealthCheck
    {
        private readonly ILogger<CourseCurrentOpportunitiesRefresh> logger;
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly ICourseSearchApiService courseSearchApiService;
        private readonly AutoMapper.IMapper mapper;
        private readonly CourseSearchSettings courseSearchSettings;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public CourseCurrentOpportunitiesRefresh(ILogger<CourseCurrentOpportunitiesRefresh> logger, ICosmosRepository<CurrentOpportunitiesSegmentModel> repository, ICourseSearchApiService courseSearchApiService, AutoMapper.IMapper mapper, CourseSearchSettings courseSearchSettings, IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService)
        {
            this.logger = logger;
            this.repository = repository;
            this.courseSearchApiService = courseSearchApiService;
            this.mapper = mapper;
            this.courseSearchSettings = courseSearchSettings;
            this.jobProfileSegmentRefreshService = jobProfileSegmentRefreshService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var description = $"{typeof(CourseCurrentOpportunitiesRefresh).Namespace} - SearchKeywords used [{courseSearchSettings.HealthCheckKeyWords}]";
            logger.LogInformation($"{nameof(CheckHealthAsync)} has been called - service {description}");

            var result = await courseSearchApiService.GetCoursesAsync(courseSearchSettings.HealthCheckKeyWords).ConfigureAwait(false);
            if (result.Any())
            {
                return HealthCheckResult.Healthy(description);
            }
            else
            {
                return HealthCheckResult.Degraded(description);
            }
        }

        public async Task<int> RefreshCoursesAndUpdateJobProfileAsync(Guid documentId)
        {
            logger.LogInformation($"{nameof(RefreshCoursesAndUpdateJobProfileAsync)} has been called for document {documentId}");
            var currentOpportunitiesSegmentModel = await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
            var numberPulled = await RefreshCoursesAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);
            var refreshJobProfileSegmentServiceBusModel = mapper.Map<RefreshJobProfileSegmentServiceBusModel>(currentOpportunitiesSegmentModel);
            await jobProfileSegmentRefreshService.SendMessageAsync(refreshJobProfileSegmentServiceBusModel).ConfigureAwait(false);
            return numberPulled;
        }

        public async Task<int> RefreshCoursesAsync(Guid documentId)
        {
            logger.LogInformation($"{nameof(RefreshCoursesAsync)} has been called for document {documentId}");
            var currentOpportunitiesSegmentModel = await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
            return await RefreshCoursesAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We want to catch all errors that happen when we call the external API")]
        private async Task<int> RefreshCoursesAsync(CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel)
        {
            logger.LogInformation($"Getting course for {currentOpportunitiesSegmentModel.CanonicalName} - course keywords {currentOpportunitiesSegmentModel.Data.Courses.CourseKeywords}");
            List<Course> courseSearchResults = new List<Course>();

            if (!string.IsNullOrWhiteSpace(currentOpportunitiesSegmentModel.Data.Courses.CourseKeywords))
            {
                try
                {
                    var results = await courseSearchApiService.GetCoursesAsync(currentOpportunitiesSegmentModel.Data.Courses.CourseKeywords, true).ConfigureAwait(false);
                    courseSearchResults = results?.ToList();
                }
                catch (Exception ex)
                {
                    var errorMessage = $"{nameof(RefreshCoursesAsync)} had error";
                    logger.LogError(ex, errorMessage);
                    throw;
                }
            }

            var opportunities = new List<Opportunity>();

            //Leaving this check in just incase the FAC client gets changed and starts to return null if no courses found.
            if (courseSearchResults != null)
            {
                foreach (var course in courseSearchResults)
                {
                    var opportunity = mapper.Map<Opportunity>(course);
                    opportunity.URL = new Uri($"{courseSearchSettings.CourseSearchUrl}/find-a-course/course-details?CourseId={opportunity.CourseId}&r={opportunity.RunId}");
                    opportunities.Add(opportunity);
                    logger.LogInformation($"{nameof(RefreshCoursesAsync)} added details for {course.CourseId} to list");
                }
            }

            currentOpportunitiesSegmentModel.Data.Courses.Opportunities = opportunities;
            await repository.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);
            return courseSearchResults == null ? 0 : courseSearchResults.Count;
        }
    }
}