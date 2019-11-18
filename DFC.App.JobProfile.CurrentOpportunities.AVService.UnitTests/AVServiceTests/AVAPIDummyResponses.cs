using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService.UnitTests
{
    public static class AVAPIDummyResponses
    {
        public static string GetDummyApprenticeshipVacancySummaryResponse(int currentPage, int totalMatches, int nunmberToReturn, int pageSize, int diffrentProvidersPage)
        {
            var r = new ApprenticeshipVacancySummaryResponse();
            r.CurrentPage = currentPage;
            r.TotalMatched = totalMatches;
            r.TotalPages = totalMatches / pageSize;
            r.TotalReturned = nunmberToReturn;

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
