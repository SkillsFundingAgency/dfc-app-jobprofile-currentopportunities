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

namespace DFC.App.JobProfile.CurrentOpportunities.AVService.UnitTests
{
    public enum Scenario
    {
        OnlySingleProviderMoreThanTwo,
        MultipeProvidersMoreThanTwo,
        MultipeProvidersMoreThanTwoStartDatesMatch,
        OnlyOneAvailable,
        NoneAvailable,
    }

    [Trait("AVCurrentOpportuntiesRefresh", "Tests")]
    public class AVCurrentOpportuntiesRefreshTests
    {
        [Theory]
        [InlineData(Scenario.OnlySingleProviderMoreThanTwo, 2)]
        [InlineData(Scenario.MultipeProvidersMoreThanTwo, 2)]
        [InlineData(Scenario.OnlyOneAvailable, 1)]
        [InlineData(Scenario.NoneAvailable, 0)]
        public void ProjectVacanciesForProfile(Scenario scenario, int expectedNumberDisplayed)
        {
            //arrange
            var fakeLogger = A.Fake<ILogger<AVCurrentOpportuntiesRefresh>>();
            var fakeCurrentOpportunitiesSegmentService = A.Fake<CurrentOpportunitiesSegmentService>();
            var fakeAVAPIService = A.Fake<AVAPIService>();
            var fakeMapper = A.Fake<AutoMapper.IMapper>();

            var aVCurrentOpportuntiesRefresh = new AVCurrentOpportuntiesRefresh(fakeLogger, fakeCurrentOpportunitiesSegmentService, fakeAVAPIService, fakeMapper);

            //Act
            var projectedVacancies = aVCurrentOpportuntiesRefresh.ProjectVacanciesForProfile(GetTestMappedVacancySummary(scenario));

            //Asserts
            CheckResultIsAsExpected(projectedVacancies, expectedNumberDisplayed);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void RefreshApprenticeshipVacanciesAsync(int numberVacanciesFound)
        {
            //arrange
            var fakeLogger = A.Fake<ILogger<AVCurrentOpportuntiesRefresh>>();
            var fakeCurrentOpportunitiesSegmentService = A.Fake<ICurrentOpportunitiesSegmentService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();
            var fakeMapper = A.Fake<AutoMapper.IMapper>();
            var currentOpportunitiesSegmentModel = new CurrentOpportunitiesSegmentModel
            {
                CanonicalName = "DummyJob",
                Data = new CurrentOpportunitiesSegmentDataModel()
                {
                    Apprenticeships = new Apprenticeships()
                    {
                        Standards = new string[] { "S1" },
                        Frameworks = new string[] { "F1" },
                    },
                },
            };

            A.CallTo(() => fakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeAVAPIService.GetAVsForMultipleProvidersAsync(A<AVMapping>.Ignored)).Returns(GetTestVacancies(numberVacanciesFound));
            A.CallTo(() => fakeAVAPIService.GetApprenticeshipVacancyDetailsAsync(A<int>.Ignored)).Returns(A.Dummy<ApprenticeshipVacancyDetails>());
            A.CallTo(() => fakeMapper.Map<Vacancy>(A<ApprenticeshipVacancyDetails>.Ignored)).Returns(A.Dummy<Vacancy>());

            var aVCurrentOpportuntiesRefresh = new AVCurrentOpportuntiesRefresh(fakeLogger, fakeCurrentOpportunitiesSegmentService, fakeAVAPIService, fakeMapper);

            //Act
            var result = aVCurrentOpportuntiesRefresh.RefreshApprenticeshipVacanciesAsync(A.Dummy<Guid>()).Result;

            //Asserts
            result.Should().Be(numberVacanciesFound);
            A.CallTo(() => fakeCurrentOpportunitiesSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAVAPIService.GetApprenticeshipVacancyDetailsAsync(A<int>.Ignored)).MustHaveHappened(numberVacanciesFound, Times.Exactly);
            A.CallTo(() => fakeCurrentOpportunitiesSegmentService.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        private static IEnumerable<ApprenticeshipVacancySummary> GetTestVacancies(int numberToGet)
        {
            for (int ii = 0; ii < numberToGet; ii++)
            {
                yield return new ApprenticeshipVacancySummary() { TrainingProviderName = "Provider A", Title = "Displayed" };
            }
        }

        private static IEnumerable<ApprenticeshipVacancySummary> GetTestVacanciesMultipeProvidersMoreThanTwo()
        {
            yield return new ApprenticeshipVacancySummary() { TrainingProviderName = "Provider A", Title = "Displayed" };
            yield return new ApprenticeshipVacancySummary() { TrainingProviderName = "Provider A", Title = "Not Displayed" };
            yield return new ApprenticeshipVacancySummary() { TrainingProviderName = "Provider B", Title = "Displayed" };
        }

        private static IEnumerable<ApprenticeshipVacancySummary> GetTestVacanciesOnlyOneAvailable()
        {
            yield return new ApprenticeshipVacancySummary() { TrainingProviderName = "Provider A", Title = "Displayed" };
        }

        private static IEnumerable<ApprenticeshipVacancySummary> GetTestVacanciesSingleProviderMoreThanTwo()
        {
            yield return new ApprenticeshipVacancySummary() { TrainingProviderName = "Provider A", Title = "Displayed" };
            yield return new ApprenticeshipVacancySummary() { TrainingProviderName = "Provider A", Title = "Displayed" };
            yield return new ApprenticeshipVacancySummary() { TrainingProviderName = "Provider A", Title = "Not Displayed" };
        }

        private void CheckResultIsAsExpected(IEnumerable<ApprenticeshipVacancySummary> projectedVacancies, int expectedCount)
        {
            int numberOfProjectedVacanices = 0;
            if (expectedCount > 0)
            {
                foreach (ApprenticeshipVacancySummary v in projectedVacancies)
                {
                    numberOfProjectedVacanices++;
                    v.Title.Should().Be("Displayed");
                }
            }

            numberOfProjectedVacanices.Should().Be(expectedCount);
        }

        private IEnumerable<ApprenticeshipVacancySummary> GetTestMappedVacancySummary(Scenario scenario)
        {
            var vacancies = Enumerable.Empty<ApprenticeshipVacancySummary>();

            switch (scenario)
            {
                case Scenario.OnlySingleProviderMoreThanTwo:
                    vacancies = GetTestVacanciesSingleProviderMoreThanTwo();
                    break;

                case Scenario.MultipeProvidersMoreThanTwo:
                    vacancies = GetTestVacanciesMultipeProvidersMoreThanTwo();
                    break;

                case Scenario.OnlyOneAvailable:
                    vacancies = GetTestVacanciesOnlyOneAvailable();
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
