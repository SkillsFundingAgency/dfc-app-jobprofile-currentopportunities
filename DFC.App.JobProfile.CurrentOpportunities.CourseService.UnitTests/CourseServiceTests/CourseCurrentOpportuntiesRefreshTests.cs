using DFC.App.FindACourseClient.Contracts;
using DFC.App.FindACourseClient.Models;
using DFC.App.FindACourseClient.Models.Configuration;
using DFC.App.JobProfile.CurrentOpportunities.CourseService;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.SegmentService;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.DraftSegmentService.UnitTests
{
    public enum Scenario
    {
        OnlySingleProviderMoreThanTwo,
        MultipeProvidersMoreThanTwo,
        MultipeProvidersMoreThanTwoStartDatesMatch,
        OnlyOneAvailable,
        NoneAvailable,
    }

    [Trait("Course Current Opportunties Refresh", "Course refresh for Jobprofiles tests")]
    public class CourseCurrentOpportuntiesRefreshTests
    {
        private ILogger<CourseCurrentOpportuntiesRefresh> fakeLogger;
        private ICurrentOpportunitiesSegmentService fakeCurrentOpportunitiesSegmentService;
        private ICourseSearchClient fakeCourseSearchClient;
        private AutoMapper.IMapper fakeMapper;
        private CourseSearchSettings courseSearchSettings;
        private CurrentOpportunitiesSegmentModel currentOpportunitiesSegmentModel;

        public CourseCurrentOpportuntiesRefreshTests()
        {
            fakeLogger = A.Fake<ILogger<CourseCurrentOpportuntiesRefresh>>();
            fakeCurrentOpportunitiesSegmentService = A.Fake<ICurrentOpportunitiesSegmentService>();
            fakeCourseSearchClient = A.Fake<ICourseSearchClient>();
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
        public void RefreshCoursesAsync(int numberVacanciesFound)
        {
            //arrange
            A.CallTo(() => fakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored)).Returns(GetTestCourses(numberVacanciesFound));
            A.CallTo(() => fakeMapper.Map<Opportunity>(A<CourseSumary>.Ignored)).Returns(new Opportunity() { CourseId = "1" });

            var courseCurrentOpportuntiesRefresh = new CourseCurrentOpportuntiesRefresh(fakeLogger, fakeCurrentOpportunitiesSegmentService, fakeCourseSearchClient, fakeMapper, courseSearchSettings);

            //Act
            var result = courseCurrentOpportuntiesRefresh.RefreshCoursesAsync(A.Dummy<Guid>()).Result;

            //Asserts
            result.NumberPulled.Should().Be(numberVacanciesFound);
            A.CallTo(() => fakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeCurrentOpportunitiesSegmentService.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RefreshCoursesAsyncNullTest()
        {
            //arrange
            A.CallTo(() => fakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeCourseSearchClient.GetCoursesAsync(A<string>.Ignored)).Throws(new ApplicationException());

            var courseCurrentOpportuntiesRefresh = new CourseCurrentOpportuntiesRefresh(fakeLogger, fakeCurrentOpportunitiesSegmentService, fakeCourseSearchClient, fakeMapper, courseSearchSettings);

            //Act
            var result = courseCurrentOpportuntiesRefresh.RefreshCoursesAsync(A.Dummy<Guid>()).Result;

            //Asserts
            result.NumberPulled.Should().Be(0);
            A.CallTo(() => fakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeCurrentOpportunitiesSegmentService.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<Opportunity>(A<object>.Ignored)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(Scenario.OnlySingleProviderMoreThanTwo, 2)]
        [InlineData(Scenario.MultipeProvidersMoreThanTwo, 2)]
        [InlineData(Scenario.OnlyOneAvailable, 1)]
        [InlineData(Scenario.NoneAvailable, 0)]
        public void SelectCoursesForProfile(Scenario scenario, int expectedNumberDisplayed)
        {
            //arrange
            A.CallTo(() => fakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            var courseCurrentOpportuntiesRefresh = new CourseCurrentOpportuntiesRefresh(fakeLogger, fakeCurrentOpportunitiesSegmentService, fakeCourseSearchClient, fakeMapper, courseSearchSettings);

            //Act
            var projectedVacancies = courseCurrentOpportuntiesRefresh.SelectCoursesForJobProfile(GetTestCourseSummaries(scenario));

            //Asserts
            CheckResultIsAsExpected(projectedVacancies, expectedNumberDisplayed);
        }


        private static IEnumerable<CourseSumary> GetTestCourses(int numberToGet)
        {
            for (int ii = 0; ii < numberToGet; ii++)
            {
                yield return new CourseSumary() { Provider = "Provider A", Title = "Displayed" };
            }
        }

        private static IEnumerable<CourseSumary> GetTestCoursesMultipeProvidersMoreThanTwo()
        {
            yield return new CourseSumary() { Provider = "Provider A", Title = "Displayed" };
            yield return new CourseSumary() { Provider = "Provider A", Title = "Not Displayed" };
            yield return new CourseSumary() { Provider = "Provider B", Title = "Displayed" };
        }

        private static IEnumerable<CourseSumary> GetTestCoursesOnlyOneAvailable()
        {
            yield return new CourseSumary() { Provider = "Provider A", Title = "Displayed" };
        }

        private static IEnumerable<CourseSumary> GetTestCoursesSingleProviderMoreThanTwo()
        {
            yield return new CourseSumary() { Provider = "Provider A", Title = "Displayed" };
            yield return new CourseSumary() { Provider = "Provider A", Title = "Displayed" };
            yield return new CourseSumary() { Provider = "Provider A", Title = "Not Displayed" };
        }

        private void CheckResultIsAsExpected(IEnumerable<CourseSumary> selectedCourses, int expectedCount)
        {
            int numberOfSelectedCourses = 0;
            if (expectedCount > 0)
            {
                foreach (CourseSumary v in selectedCourses)
                {
                    numberOfSelectedCourses++;
                    v.Title.Should().Be("Displayed");
                }
            }

            numberOfSelectedCourses.Should().Be(expectedCount);
        }

        private IEnumerable<CourseSumary> GetTestCourseSummaries(Scenario scenario)
        {
            var vacancies = Enumerable.Empty<CourseSumary>();

            switch (scenario)
            {
                case Scenario.OnlySingleProviderMoreThanTwo:
                    vacancies = GetTestCoursesSingleProviderMoreThanTwo();
                    break;

                case Scenario.MultipeProvidersMoreThanTwo:
                    vacancies = GetTestCoursesMultipeProvidersMoreThanTwo();
                    break;

                case Scenario.OnlyOneAvailable:
                    vacancies = GetTestCoursesOnlyOneAvailable();
                    break;

                case Scenario.NoneAvailable:
                    break;

                default:
                    throw new InvalidOperationException("Test Scenarios not supported");
            }

            return vacancies;
        }
    }
}
