using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace DFC.App.JobProfile.CurrentOpportunities.IntegrationTests
{
    public static class DataSeeding
    {
        public const string DefaultArticleName = "segment-article";

        public static Guid DefaultArticleGuid => Guid.Parse("63DEA97E-B61C-4C14-15DC-1BD08EA20DC8");

        public static void SeedDefaultArticle(CustomWebApplicationFactory<Startup> factory)
        {
            const string url = "/segment";
            var models = new List<CurrentOpportunitiesSegmentModel>()
            {
                 GetDummyCurrentOpportunitiesSegmentModel(DefaultArticleGuid, DefaultArticleName, 1),

                 //GetDummyCurrentOpportunitiesSegmentModel(Guid.Parse("C16B389D-91AD-4F3D-2485-9F7EE953AFE4"), $"{DefaultArticleName}-2", new DateTime(2019, 09, 02, 12, 13, 24), 2),
                 //GetDummyCurrentOpportunitiesSegmentModel(Guid.Parse("C0103C26-E7C9-4008-3F66-1B2DB192177E"), $"{DefaultArticleName}-3", new DateTime(2018, 08, 12, 15, 20, 10), 2),
            };

            var client = factory?.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            models.ForEach(f => client.PostAsync(url, f, new JsonMediaTypeFormatter()).GetAwaiter().GetResult());
        }

        public static CurrentOpportunitiesSegmentModel GetDummyCurrentOpportunitiesSegmentModel(Guid documentId, string canonicalName, int dataIndex)
        {
            return new CurrentOpportunitiesSegmentModel()
            {
                DocumentId = documentId,
                CanonicalName = canonicalName,
                SocLevelTwo = $"0{dataIndex}",
                Data = GetDummyCurrentOpportunitiesSegmentModel(dataIndex),
            };
        }

        private static CurrentOpportunitiesSegmentDataModel GetDummyCurrentOpportunitiesSegmentModel(int index)
        {
            return new CurrentOpportunitiesSegmentDataModel()
            {
                LastReviewed = DateTime.UtcNow,
                JobTitle = $"JobProfile{index}",
                Apprenticeships = new Apprenticeships() //Standards and frameworks should be valid or integration tests will fail
                {
                    Standards = new List<ApprenticeshipStandard>()
                    {
                        new ApprenticeshipStandard
                        {
                            Url = "25",
                        },
                        new ApprenticeshipStandard
                        {
                            Url = "36",
                        },
                    },
                    Frameworks = new List<ApprenticeshipFramework>(),
                    Vacancies = new List<Vacancy>()
                    {
                        new Vacancy { ApprenticeshipId = $"1{index}", Location = new Location() { PostCode = $"PC1{index}", Town = $"Location1{index}" }, Title = $"Title1{index}", WageUnit = $"£10{index}", WageText = $"Per 1{index} days" },
                        new Vacancy { ApprenticeshipId = $"2{index}", Location = new Location() { PostCode = $"PC2{index}", Town = $"Location2{index}" }, Title = $"Title2{index}", WageUnit = $"£10{index}", WageText = $"Per 2{index} days" },
                    },
                },
                Courses = new Courses()
                {
                    CourseKeywords = $"CousreKeyword{index}",
                    Opportunities = new List<Opportunity>()
                    {
                        new Opportunity() { CourseId = $"1{index}", Location = new Location() { PostCode = $"PC1{index}", Town = $"Location1{index}" }, Provider = $"Provider1{index}", Title = $"Title1{index}" },
                        new Opportunity() { CourseId = $"2{index}", Location = new Location() { PostCode = $"PC1{index}", Town = $"Location1{index}" }, Provider = $"Provider2{index}", Title = $"Title2{index}" },
                    },
                },
            };
        }
    }
}
