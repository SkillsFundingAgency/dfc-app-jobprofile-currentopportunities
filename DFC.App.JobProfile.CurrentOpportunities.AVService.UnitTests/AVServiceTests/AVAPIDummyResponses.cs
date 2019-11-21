using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService.UnitTests
{
    public static class AVAPIDummyResponses
    {
        public static string GetDummyApprenticeshipVacancySummaryResponse(int currentPage, int totalMatches, int nunmberToReturn, int pageSize, int diffrentProvidersPage)
        {
            var r = new ApprenticeshipVacancySummaryResponse
            {
                CurrentPage = currentPage,
                TotalMatched = totalMatches,
                TotalPages = totalMatches / pageSize,
                TotalReturned = nunmberToReturn,
            };

            var recordsToReturn = new List<ApprenticeshipVacancySummary>();

            for (int ii = 0; ii < nunmberToReturn; ii++)
            {
                recordsToReturn.Add(new ApprenticeshipVacancySummary()
                {
                    VacancyReference = ii,
                    Title = $"Title {ii}",
                    TrainingProviderName = $"TrainingProviderName {((currentPage == diffrentProvidersPage) ? ii : currentPage)}",
                });
            }

            r.Results = recordsToReturn.ToArray();

            return JsonConvert.SerializeObject(r);
        }

        public static string GetDummyApprenticeshipVacancyDetailsResponse()
        {
            var r = new ApprenticeshipVacancyDetails()
            {
                VacancyReference = 123,
            };

            return JsonConvert.SerializeObject(r);
        }
    }
}
