using DFC.App.FindACourseClient.Contracts;
using DFC.App.FindACourseClient.Models;
using DFC.App.FindACourseClient.Models.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.CourseService
{
    public class CourseCurrentOpportuntiesRefresh : ICourseCurrentOpportuntiesRefresh, IHealthCheck
    {
        private readonly ILogger<CourseCurrentOpportuntiesRefresh> logger;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly ICourseSearchClient courseSearch;
        private readonly AutoMapper.IMapper mapper;
        private readonly CourseSearchSettings courseSearchSettings;

        public CourseCurrentOpportuntiesRefresh(ILogger<CourseCurrentOpportuntiesRefresh> logger, ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService, ICourseSearchClient courseSearch, AutoMapper.IMapper mapper, CourseSearchSettings courseSearchSettings)
        {
            this.logger = logger;
            this.currentOpportunitiesSegmentService = currentOpportunitiesSegmentService;
            this.courseSearch = courseSearch;
            this.mapper = mapper;
            this.courseSearchSettings = courseSearchSettings;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var description = $"{typeof(CourseCurrentOpportuntiesRefresh).Namespace} - SearchKeywords used [{courseSearchSettings.HealthCheckKeyWords}]";
            logger.LogInformation($"{nameof(CheckHealthAsync)} has been called - service {description}");

            var result = await courseSearch.GetCoursesAsync(courseSearchSettings.HealthCheckKeyWords).ConfigureAwait(false);
            if (result.Any())
            {
                return HealthCheckResult.Healthy(description);
            }
            else
            {
                return HealthCheckResult.Degraded(description);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We want to catch all errors that happen when we call the external API")]
        public async Task<FeedRefreshResponseModel> RefreshCoursesAsync(Guid documentId)
        {
            var feedRefreshResponseModel = new FeedRefreshResponseModel() { NumberPulled = 0 };

            logger.LogInformation($"{nameof(RefreshCoursesAsync)} has been called for document {documentId}");
            CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel = await currentOpportunitiesSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            IEnumerable<CourseSumary> courseSearchResults = Enumerable.Empty<CourseSumary>();
            logger.LogInformation($"Getting course for {currentOpportunitiesSegmentModel.CanonicalName} - course keywords {currentOpportunitiesSegmentModel.Data.Courses.CourseKeywords}");

            //if the the call to the courses API fails for anyreason we should log and continue as if there are no courses available.
            try
            {
                courseSearchResults = await courseSearch.GetCoursesAsync(currentOpportunitiesSegmentModel.Data.Courses.CourseKeywords).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var errorMessge = $"{nameof(RefreshCoursesAsync)} had error";
                logger.LogError(ex, errorMessge);
                throw;
            }

            var selectedCourses = SelectCoursesForJobProfile(courseSearchResults);
            var opportunities = new List<Opportunity>();
            foreach (var course in selectedCourses)
            {
                var opportunity = mapper.Map<Opportunity>(course);
                opportunity.URL = new Uri($"{courseSearchSettings.CourseSearchUrl}{opportunity.CourseId}");
                opportunities.Add(opportunity);
                logger.LogInformation($"{nameof(RefreshCoursesAsync)} added details for {course.CourseId} to list");
            }

            currentOpportunitiesSegmentModel.Data.Courses.Opportunities = opportunities;
            await currentOpportunitiesSegmentService.UpsertAsync(currentOpportunitiesSegmentModel).ConfigureAwait(false);
            feedRefreshResponseModel.NumberPulled = selectedCourses.Count();
            return feedRefreshResponseModel;
        }

        public IEnumerable<CourseSumary> SelectCoursesForJobProfile(IEnumerable<CourseSumary> courses)
        {
            if (courses == null)
            {
                return Enumerable.Empty<CourseSumary>();
            }

            if (courses.Count() > 2)
            {
                var distinctProviders = courses.Select(c => c.Provider).Distinct().Count();
                if (distinctProviders > 1)
                {
                    return courses
                            .GroupBy(c => c.Provider)
                            .Select(g => g.First())
                            .Take(2);
                }
            }

            return courses.Take(2);
        }
    }
}
