using DFC.App.FindACourseClient.Models.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.FindACourseClient;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.CourseService.UnitTests
{
    [Trait("Course Current Opportunities Refresh", "Health Status Tests")]
    public class CourseServiceHealthStatusCheckTests
    {
        private readonly Data.Contracts.ICosmosRepository<CurrentOpportunitiesSegmentModel> fakeRepository;
        private readonly ICourseSearchApiService fakeCourseSearchClient;
        private readonly AutoMapper.IMapper fakeMapper;
        private readonly CourseSearchSettings courseSearchSettings;
        private readonly ILogger<CourseCurrentOpportunitiesRefresh> fakeLogger;
        private readonly HealthCheckContext dummyHealthCheckContext;

        public CourseServiceHealthStatusCheckTests()
        {
            fakeLogger = A.Fake<ILogger<CourseCurrentOpportunitiesRefresh>>();
            dummyHealthCheckContext = A.Dummy<HealthCheckContext>();
            fakeLogger = A.Fake<ILogger<CourseCurrentOpportunitiesRefresh>>();
            fakeRepository = A.Fake<Data.Contracts.ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            fakeCourseSearchClient = A.Fake<ICourseSearchApiService>();
            fakeMapper = A.Fake<AutoMapper.IMapper>();
            courseSearchSettings = new CourseSearchSettings()
            {
                HealthCheckKeyWords = "DummyKeyword",
            };
        }

        [Theory]
        [InlineData(5, HealthStatus.Healthy)]
        [InlineData(0, HealthStatus.Degraded)]
        public async Task GetCurrentHealthStatusAsyncTestAsync(int recordsToReturn, HealthStatus expectedStatus)
        {
            //Arrange
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored)).Returns(GetTestCourses(recordsToReturn));
            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings);

            //Act
            var serviceHealthStatus = await courseCurrentOpportunitiesRefresh.CheckHealthAsync(dummyHealthCheckContext).ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.Status.Should().Be(expectedStatus);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>._)).MustHaveHappened();
        }

        [Fact]
        public void GetCurrentHealthStatusAsyncExceptionTestAsync()
        {
            //Arrange
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored)).Throws(new ApplicationException());
            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings);

            //Act
            Func<Task> serviceHealthStatus = async () => await courseCurrentOpportunitiesRefresh.CheckHealthAsync(dummyHealthCheckContext).ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.Should().Throw<Exception>();
        }

        private static IEnumerable<Course> GetTestCourses(int numberToGet)
        {
            for (int ii = 0; ii < numberToGet; ii++)
            {
                yield return new Course() { ProviderName = "Provider A", Title = "Displayed" };
            }
        }
    }
}