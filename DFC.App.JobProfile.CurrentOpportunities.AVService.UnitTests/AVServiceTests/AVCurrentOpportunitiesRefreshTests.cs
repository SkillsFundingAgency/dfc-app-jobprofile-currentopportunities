using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    [Trait("AVCurrentOpportunitiesRefresh", "Tests")]
    public class AVCurrentOpportunitiesRefreshTests
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
            var fakeRepository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            var fakeAVAPIService = A.Fake<AVAPIService>();
            var fakeMapper = A.Fake<AutoMapper.IMapper>();

            var aVCurrentOpportunitiesRefresh = new AVCurrentOpportuntiesRefresh(fakeLogger, fakeRepository, fakeAVAPIService, fakeMapper);

            //Act
            var projectedVacancies = aVCurrentOpportunitiesRefresh.ProjectVacanciesForProfile(GetTestMappedVacancySummary(scenario));

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
            var fakeRepository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();
            var fakeMapper = A.Fake<AutoMapper.IMapper>();
            var currentOpportunitiesSegmentModel = new CurrentOpportunitiesSegmentModel
            {
                CanonicalName = "DummyJob",
                Data = new CurrentOpportunitiesSegmentDataModel()
                {
                    Apprenticeships = new Apprenticeships()
                    {
                        Standards = new List<ApprenticeshipStandard>()
                        {
                            new ApprenticeshipStandard
                            {
                                Url = "S1",
                            },
                        },
                        Frameworks = new List<ApprenticeshipFramework>()
                        {
                            new ApprenticeshipFramework
                            {
                                Url = "F1",
                            },
                        },
                    },
                },
            };

            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeAVAPIService.GetAVsForMultipleProvidersAsync(A<AVMapping>.Ignored)).Returns(GetTestVacancies(numberVacanciesFound));
            A.CallTo(() => fakeAVAPIService.GetApprenticeshipVacancyDetailsAsync(A<int>.Ignored)).Returns(A.Dummy<ApprenticeshipVacancyDetails>());
            A.CallTo(() => fakeMapper.Map<Vacancy>(A<ApprenticeshipVacancyDetails>.Ignored)).Returns(A.Dummy<Vacancy>());

            var aVCurrentOpportunitiesRefresh = new AVCurrentOpportuntiesRefresh(fakeLogger, fakeRepository, fakeAVAPIService, fakeMapper);

            //Act
            var result = aVCurrentOpportunitiesRefresh.RefreshApprenticeshipVacanciesAsync(A.Dummy<Guid>()).Result;

            //Asserts
            result.Should().Be(numberVacanciesFound);
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAVAPIService.GetApprenticeshipVacancyDetailsAsync(A<int>.Ignored)).MustHaveHappened(numberVacanciesFound, Times.Exactly);
            A.CallTo(() => fakeRepository.UpsertAsync(A<CurrentOpportunitiesSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("F1", "S1", true)]
        [InlineData("", "S1", true )]
        [InlineData("F1", "", true)]
        [InlineData("", "", false)]
        public void BlankStandardsAndFrameworksAsync(string framework, string standard, bool requestShouldBeMade)
        {
            //arrange
            var fakeLogger = A.Fake<ILogger<AVCurrentOpportuntiesRefresh>>();
            var fakeRepository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();
            var fakeMapper = A.Fake<AutoMapper.IMapper>();
            var currentOpportunitiesSegmentModel = new CurrentOpportunitiesSegmentModel
            {
                CanonicalName = "DummyJob",
                Data = new CurrentOpportunitiesSegmentDataModel()
                {
                    Apprenticeships = new Apprenticeships()
                    {
                        Standards = new List<ApprenticeshipStandard>()
                        {
                            new ApprenticeshipStandard
                            {
                                Url = standard,
                            },
                        },
                        Frameworks = new List<ApprenticeshipFramework>()
                        {
                            new ApprenticeshipFramework
                            {
                                Url = framework,
                            },
                        },
                    },
                },
            };

            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<CurrentOpportunitiesSegmentModel, bool>>>.Ignored)).Returns(currentOpportunitiesSegmentModel);
            A.CallTo(() => fakeAVAPIService.GetAVsForMultipleProvidersAsync(A<AVMapping>.Ignored)).Returns(GetTestVacancies(2));
            A.CallTo(() => fakeAVAPIService.GetApprenticeshipVacancyDetailsAsync(A<int>.Ignored)).Returns(A.Dummy<ApprenticeshipVacancyDetails>());
            A.CallTo(() => fakeMapper.Map<Vacancy>(A<ApprenticeshipVacancyDetails>.Ignored)).Returns(A.Dummy<Vacancy>());

            var aVCurrentOpportunitiesRefresh = new AVCurrentOpportuntiesRefresh(fakeLogger, fakeRepository, fakeAVAPIService, fakeMapper);

            //Act
            var result = aVCurrentOpportunitiesRefresh.RefreshApprenticeshipVacanciesAsync(A.Dummy<Guid>()).Result;

            //Asserts
            if (requestShouldBeMade)
            {
                result.Should().Be(2);
                A.CallTo(() => fakeAVAPIService.GetAVsForMultipleProvidersAsync(A<AVMapping>.Ignored)).MustHaveHappenedOnceExactly();
            }
            else
            {
                result.Should().Be(0);
                A.CallTo(() => fakeAVAPIService.GetAVsForMultipleProvidersAsync(A<AVMapping>.Ignored)).MustNotHaveHappened();
            }
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