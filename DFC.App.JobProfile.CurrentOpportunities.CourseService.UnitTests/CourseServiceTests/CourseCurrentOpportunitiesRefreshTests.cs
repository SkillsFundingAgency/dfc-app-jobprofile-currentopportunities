using DFC.App.FindACourseClient.Models.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using DFC.App.JobProfile.CurrentOpportunities.SegmentService;
using DFC.FindACourseClient;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.CourseService.UnitTests
{
    public enum Scenario
    {
        OnlySingleProviderMoreThanTwo,
        MultipeProvidersMoreThanTwo,
        MultipeProvidersMoreThanTwoStartDatesMatch,
        OnlyOneAvailable,
        NoneAvailable,
    }

    [Trait("Course Current Opportunities Refresh", "Course refresh for Jobprofiles tests")]
    public class CourseCurrentOpportunitiesRefreshTests
    {
        private readonly ILogger<CourseCurrentOpportunitiesRefresh> fakeLogger;
        private readonly Data.Contracts.ICosmosRepository<CurrentOpportunitiesSegmentModel> fakeRepository;
        private readonly ICourseSearchApiService fakeCourseSearchClient;
        private readonly AutoMapper.IMapper fakeMapper;
        private readonly CourseSearchSettings courseSearchSettings;
        private readonly CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> fakejobProfileSegmentRefreshService;

        public CourseCurrentOpportunitiesRefreshTests()
        {
            fakeLogger = A.Fake<ILogger<CourseCurrentOpportunitiesRefresh>>();
            fakeRepository = A.Fake<Data.Contracts.ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            fakeCourseSearchClient = A.Fake<ICourseSearchApiService>();
            fakeMapper = A.Fake<AutoMapper.IMapper>();
            fakejobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();

            courseSearchSettings = new CourseSearchSettings()
            {
                CourseSearchUrl = new Uri("htpp://www.test.com/"),
            };

            currentOpportunitiesSegmentModel = new CurrentOpportunitiesSegmentModel
            {
                CanonicalName = "DummyJob",
                Data = new CurrentOpportunitiesSegmentDataModel()
                {
                    Courses = new Courses()
                    {
                        CourseKeywords = "dummyKeyword",
                    },
                },
            };
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task RefreshCoursesAsync(int numberVacanciesFound)
        {
            //Arrange
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored, true)).Returns(GetTestCourses(numberVacanciesFound));
            A.CallTo(() => fakeMapper.Map<Opportunity>(A<Course>.Ignored)).Returns(new Opportunity() { CourseId = Guid.NewGuid().ToString(), TLevelId = Guid.NewGuid().ToString() });

            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings, fakejobProfileSegmentRefreshService);

            //Act
            var result = await courseCurrentOpportunitiesRefresh.RefreshCoursesAsync(A.Dummy<Guid>()).ConfigureAwait(false);

            //Asserts
            result.Should().Be(numberVacanciesFound);
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RefreshCoursesAsyncUpsertsCoursesWithCourseDetailsUrlsWhenCoursesAreNotTLevel()
        {
            //Arrange
            int numberVacanciesFound = 2;

            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored, true)).Returns(GetTestCourses(numberVacanciesFound));
            A.CallTo(() => fakeMapper.Map<Opportunity>(A<Course>.Ignored)).Returns(new Opportunity()
            {
                CourseId = Guid.NewGuid().ToString(),
                RunId = Guid.NewGuid().ToString(),
                TLevelId = Guid.Empty.ToString(),
            });

            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings, fakejobProfileSegmentRefreshService);

            //Act
            var result = await courseCurrentOpportunitiesRefresh.RefreshCoursesAsync(A.Dummy<Guid>()).ConfigureAwait(false);

            //Asserts
            result.Should().Be(numberVacanciesFound);

            var opportunities = currentOpportunitiesSegmentModel.Data.Courses.Opportunities;
            opportunities.Should().NotBeNull();
            var opportunity1 = opportunities.ToList().FirstOrDefault();
            var opportunity2 = opportunities.ToList().Skip(1).FirstOrDefault();

            opportunity1.URL.ToString().Should().Contain($"{courseSearchSettings.CourseSearchUrl}find-a-course/course-details?CourseId={opportunity1.CourseId}&r={opportunity1.RunId}");
            opportunity2.URL.ToString().Should().Contain($"{courseSearchSettings.CourseSearchUrl}find-a-course/course-details?CourseId={opportunity2.CourseId}&r={opportunity2.RunId}");
        }

        [Fact]
        public async Task RefreshCoursesAsyncUpsertsCoursesWithTLevelDetailsUrlsWhenCoursesAreTLevel()
        {
            //Arrange
            int numberVacanciesFound = 2;
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored, true)).Returns(GetTestTLevelCourses(numberVacanciesFound));
            A.CallTo(() => fakeMapper.Map<Opportunity>(A<Course>.Ignored)).Returns(new Opportunity()
            {
                CourseId = Guid.Empty.ToString(),
                TLevelId = Guid.NewGuid().ToString(),
                TLevelLocationId = Guid.NewGuid().ToString(),
            });

            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings, fakejobProfileSegmentRefreshService);

            //Act
            var result = await courseCurrentOpportunitiesRefresh.RefreshCoursesAsync(A.Dummy<Guid>()).ConfigureAwait(false);

            //Asserts
            result.Should().Be(numberVacanciesFound);

            var opportunities = currentOpportunitiesSegmentModel.Data.Courses.Opportunities;
            opportunities.Should().NotBeNull();
            var opportunity1 = opportunities.ToList().FirstOrDefault();
            var opportunity2 = opportunities.ToList().Skip(1).FirstOrDefault();

            opportunity1.URL.ToString().Should().Contain($"{courseSearchSettings.CourseSearchUrl}find-a-course/tdetails?tlevelId={opportunity1.TLevelId}&tlevelLocationId={opportunity1.TLevelLocationId}");
            opportunity2.URL.ToString().Should().Contain($"{courseSearchSettings.CourseSearchUrl}find-a-course/tdetails?tlevelId={opportunity2.TLevelId}&tlevelLocationId={opportunity2.TLevelLocationId}");
        }

        [Fact]
        public void RefreshCoursesAsyncExceptionTest()
        {
            //Arrange
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored, true)).Throws(new ApplicationException());

            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings, fakejobProfileSegmentRefreshService);
            Func<Task> serviceHealthStatus = async () => await courseCurrentOpportunitiesRefresh.RefreshCoursesAsync(A.Dummy<Guid>()).ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.Should().Throw<ApplicationException>();

            //Asserts
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeMapper.Map<Opportunity>(A<object>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ShouldNotMakeACallToFACClientIfCousreKeyWordIsBlank()
        {
            //arrange
            var modelWithBlankCourses = currentOpportunitiesSegmentModel;
            modelWithBlankCourses.Data.Courses.CourseKeywords = string.Empty;
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings, fakejobProfileSegmentRefreshService);

            //Act
            var result = await courseCurrentOpportunitiesRefresh.RefreshCoursesAsync(A.Dummy<Guid>()).ConfigureAwait(false);

            //Asserts
            result.Should().Be(0);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored, true)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ShouldHandleFACClientReturningNullForASearch()
        {
            //arrange
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored, false)).Returns(GetTestCourses(0));

            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings, fakejobProfileSegmentRefreshService);

            //Act
            var result = await courseCurrentOpportunitiesRefresh.RefreshCoursesAsync(A.Dummy<Guid>()).ConfigureAwait(false);

            //Asserts
            result.Should().Be(0);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored, true)).MustHaveHappened();
        }

        [Fact]
        public async Task RefreshApprenticeshipVacanciesAndUpdateJobProfileSendsMsg()
        {
            //arrange
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);

            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings, fakejobProfileSegmentRefreshService);

            //Act
            _ = await courseCurrentOpportunitiesRefresh.RefreshCoursesAndUpdateJobProfileAsync(A.Dummy<Guid>()).ConfigureAwait(false);

            //Asserts
            A.CallTo(() => fakejobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        private static IEnumerable<Course> GetTestCourses(int numberToGet)
        {
            for (var i = 0; i < numberToGet; i++)
            {
                yield return new Course
                {
                    CourseId = Guid.NewGuid().ToString(),
                    RunId = Guid.NewGuid().ToString(),
                    TLevelId = Guid.Empty.ToString(),
                    ProviderName = "Provider A",
                    Title = "Displayed",
                };
            }
        }

        private static IEnumerable<Course> GetTestTLevelCourses(int numberToGet)
        {
            for (var i = 0; i < numberToGet; i++)
            {
                yield return new Course
                {
                    CourseId = Guid.Empty.ToString(),
                    RunId = Guid.Empty.ToString(),
                    TLevelId = Guid.NewGuid().ToString(),
                    TLevelLocationId = Guid.NewGuid().ToString(),
                    ProviderName = "Provider A",
                    Title = "T Level Course",
                };
            }
        }
    }
}