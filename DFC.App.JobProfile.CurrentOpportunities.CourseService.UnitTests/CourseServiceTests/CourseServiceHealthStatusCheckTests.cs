using DFC.App.FindACourseClient.Models.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.FindACourseClient.Contracts;
using DFC.FindACourseClient.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.CourseService.UnitTests
{
    [Trait("Course Current Opportunties Refresh", "Health Status Tests")]
    public class CourseServiceHealthStatusCheckTests
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> fakeRepository;
        private readonly ICourseSearchClient fakeCourseSearchClient;
        private readonly AutoMapper.IMapper fakeMapper;
        private readonly CourseSearchSettings courseSearchSettings;
        private readonly ILogger<CourseCurrentOpportuntiesRefresh> fakeLogger;
        private readonly HealthCheckContext dummyHealthCheckContext;

        public CourseServiceHealthStatusCheckTests()
        {
            fakeLogger = A.Fake<ILogger<CourseCurrentOpportuntiesRefresh>>();
            dummyHealthCheckContext = A.Dummy<HealthCheckContext>();
            fakeLogger = A.Fake<ILogger<CourseCurrentOpportuntiesRefresh>>();
            fakeRepository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();

            fakeRepository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            fakeCourseSearchClient = A.Fake<ICourseSearchClient>();
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
            var courseCurrentOpportuntiesRefresh = new CourseCurrentOpportuntiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings);

            //Act
            var serviceHealthStatus = await courseCurrentOpportuntiesRefresh.CheckHealthAsync(dummyHealthCheckContext).ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.Status.Should().Be(expectedStatus);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>._)).MustHaveHappened();
        }

        [Fact]
        public void GetCurrentHealthStatusAsyncExceptionTestAsync()
        {
            //Arrange
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored)).Throws(new ApplicationException());
            var courseCurrentOpportuntiesRefresh = new CourseCurrentOpportuntiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings);

            //Act
            Func<Task> serviceHealthStatus = async () => await courseCurrentOpportuntiesRefresh.CheckHealthAsync(dummyHealthCheckContext).ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.Should().Throw<Exception>();
        }

        private static IEnumerable<CourseSumary> GetTestCourses(int numberToGet)
        {
            for (int ii = 0; ii < numberToGet; ii++)
            {
                yield return new CourseSumary() { Provider = "Provider A", Title = "Displayed" };
            }
        }
    }
}