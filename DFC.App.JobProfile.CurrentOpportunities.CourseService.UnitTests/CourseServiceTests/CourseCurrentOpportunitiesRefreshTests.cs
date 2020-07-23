using DFC.App.FindACourseClient.Models.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.FindACourseClient;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public CourseCurrentOpportunitiesRefreshTests()
        {
            fakeLogger = A.Fake<ILogger<CourseCurrentOpportunitiesRefresh>>();
            fakeRepository = A.Fake<Data.Contracts.ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            fakeCourseSearchClient = A.Fake<ICourseSearchApiService>();
            fakeMapper = A.Fake<AutoMapper.IMapper>();

            courseSearchSettings = new CourseSearchSettings()
            {
                CourseSearchUrl = new Uri("htpp:\\test.com"),
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
            //arrange
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored)).Returns(GetTestCourses(numberVacanciesFound));
            A.CallTo(() => fakeMapper.Map<Opportunity>(A<Course>.Ignored)).Returns(new Opportunity() { CourseId = "1" });

            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings);

            //Act
            var result = await courseCurrentOpportunitiesRefresh.RefreshCoursesAsync(A.Dummy<Guid>()).ConfigureAwait(false);

            //Asserts
            result.Should().Be(numberVacanciesFound);
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RefreshCoursesAsyncExceptionTest()
        {
            //Arrange
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored)).Throws(new ApplicationException());

            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings);
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
            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings);

            //Act
            var result = await courseCurrentOpportunitiesRefresh.RefreshCoursesAsync(A.Dummy<Guid>()).ConfigureAwait(false);

            //Asserts
            result.Should().Be(0);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored)).MustNotHaveHappened(); 
        }

        [Fact]
        public async Task ShouldHandleFACClientReturningNullForASearch()
        {
            //arrange
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored)).Returns(GetTestCourses(0));

            var courseCurrentOpportunitiesRefresh = new CourseCurrentOpportunitiesRefresh(fakeLogger, fakeRepository, fakeCourseSearchClient, fakeMapper, courseSearchSettings);

            //Act
            var result = await courseCurrentOpportunitiesRefresh.RefreshCoursesAsync(A.Dummy<Guid>()).ConfigureAwait(false);

            //Asserts
            result.Should().Be(0);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored)).MustHaveHappened();
        }


        private static IEnumerable<Course> GetTestCourses(int numberToGet)
        {
            for (var i = 0; i < numberToGet; i++)
            {
                yield return new Course { ProviderName = "Provider A", Title = "Displayed" };
            }
        }
    }
}