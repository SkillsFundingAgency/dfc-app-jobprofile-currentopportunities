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

        public static DateTime DefaultArticleCreated => new DateTime(2019, 09, 01, 12, 13, 14);

        public static void SeedDefaultArticle(CustomWebApplicationFactory<Startup> factory)
        {
            const string url = "/segment";
            var models = new List<CurrentOpportunitiesSegmentModel>()
            {
                new CurrentOpportunitiesSegmentModel()
                {
                    DocumentId = DefaultArticleGuid,
                    CanonicalName = DefaultArticleName,
                    Created = DefaultArticleCreated,
                    Data = GetDummyCurrentOpportunitiesSegmentModel(1),
                },
                new CurrentOpportunitiesSegmentModel()
                {
                    DocumentId = Guid.Parse("C16B389D-91AD-4F3D-2485-9F7EE953AFE4"),
                    CanonicalName = $"{DefaultArticleName}-2",
                    Created = new DateTime(2019, 09, 02, 12, 13, 24),
                    Data = GetDummyCurrentOpportunitiesSegmentModel(2),
                },
                new CurrentOpportunitiesSegmentModel()
                {
                    DocumentId = Guid.Parse("C0103C26-E7C9-4008-3F66-1B2DB192177E"),
                    CanonicalName = $"{DefaultArticleName}-3",
                    Created = new DateTime(2019, 09, 03, 12, 13, 34),
                    Data = GetDummyCurrentOpportunitiesSegmentModel(3),
                },
            };

            var client = factory?.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            models.ForEach(f => client.PostAsync(url, f, new JsonMediaTypeFormatter()).GetAwaiter().GetResult());
        }

        private static CurrentOpportunitiesSegmentData GetDummyCurrentOpportunitiesSegmentModel(int index)
        {
            return new CurrentOpportunitiesSegmentData()
            {
                Updated = DateTime.UtcNow,
                JobTitle = $"JobProfile{index}",
                LastReviewed = DateTime.UtcNow,
                Standards = new string[] { $"S1{index}", $"S2{index}" },
                Frameworks = new string[] { $"F1{index}", $"F2{index}" },
                CourseKeywords = $"CousreKeyword{index}",
                Apprenticeships = new List<Apprenticeship>()
                {
                    new Apprenticeship { ApprenticeshipId = $"1{index}", LocationPostCode = $"PC1{index}", LocationTown = $"Location1{index}", Title = $"Title1{index}", WageUnit = $"£10{index}", WageText = $"Per 1{index} days" },
                    new Apprenticeship { ApprenticeshipId = $"2{index}", LocationPostCode = $"PC2{index}", LocationTown = $"Location2{index}", Title = $"Title2{index}", WageUnit = $"£10{index}", WageText = $"Per 2{index} days" },
                },
                Courses = new List<Course>()
                {
                    new Course() { CourseId = $"1{index}", Location = $"Location1{index}", Provider = $"Provider1{index}", Title = $"Title1{index}" },
                    new Course() { CourseId = $"2{index}", Location = $"Location2{index}", Provider = $"Provider2{index}", Title = $"Title2{index}" },
                },
            };
        }
    }
}
