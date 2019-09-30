using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.IntegrationTests.ControllerTests
{
    [Trait("Integration Tests", "Segment Controller Tests")]
    public class SegmentControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private const string DefaultArticleName = "segment-article";

        private readonly CustomWebApplicationFactory<Startup> factory;

        public SegmentControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            DataSeeding.SeedDefaultArticle(factory);
        }

        public static IEnumerable<object[]> SegmentContentRouteData => new List<object[]>
        {
            new object[] { "/Segment", "text/html" },
            new object[] { $"/Segment/{DefaultArticleName}", "text/html" },
            new object[] { $"/Segment/{DefaultArticleName}/contents", "text/html" },
            new object[] { $"/Segment/{DefaultArticleName}/contents", "application/json" },
        };

        public static IEnumerable<object[]> MissingSegmentContentRouteData => new List<object[]>
        {
            new object[] { $"/Segment/invalid-segment-name" },
        };

        [Theory]
        [MemberData(nameof(SegmentContentRouteData))]
        public async Task GetSegmentHtmlContentEndpointsReturnSuccessAndCorrectContentType(string url, string acceptType)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptType));

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"{acceptType}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [MemberData(nameof(MissingSegmentContentRouteData))]
        public async Task GetSegmentHtmlContentEndpointsReturnNoContent(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task PostSegmentEndpointsReturnCreated()
        {
            // Arrange
            const string url = "/segment";
            var documentId = Guid.NewGuid();
            var currentOpportunitiesSegmentModel = DataSeeding.GetDummyCurrentOpportunitiesSegmentModel(documentId, documentId.ToString().ToLowerInvariant(), DateTime.Now, 1);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.PostAsync(url, currentOpportunitiesSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task PostSegmentEndpointsForDefaultArticleRefreshAllReturnOk()
        {
            // Arrange
            const string url = "/segment";
            var currentOpportunitiesSegmentModel = DataSeeding.GetDummyCurrentOpportunitiesSegmentModel(DataSeeding.DefaultArticleGuid, DataSeeding.DefaultArticleName, DataSeeding.DefaultArticleCreated, 1);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.PostAsync(url, currentOpportunitiesSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PutSegmentEndpointsReturnOk()
        {
            // Arrange
            const string url = "/segment";
            var documentId = Guid.NewGuid();
            var currentOpportunitiesSegmentModel = DataSeeding.GetDummyCurrentOpportunitiesSegmentModel(documentId, documentId.ToString().ToLowerInvariant(), DateTime.Now, 1);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            _ = await client.PostAsync(url, currentOpportunitiesSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Act
            var response = await client.PutAsync(url, currentOpportunitiesSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteSegmentEndpointsReturnSuccessWhenFound()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            const string postUrl = "/segment";
            var deleteUri = new Uri($"/segment/{documentId}", UriKind.Relative);
            var currentOpportunitiesSegmentModel = DataSeeding.GetDummyCurrentOpportunitiesSegmentModel(documentId, documentId.ToString().ToLowerInvariant(), DateTime.Now, 1);
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            _ = await client.PostAsync(postUrl, currentOpportunitiesSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Act
            var response = await client.DeleteAsync(deleteUri).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteSegmentEndpointsReturnNotFound()
        {
            // Arrange
            var deleteUri = new Uri($"/segment/{Guid.NewGuid()}", UriKind.Relative);
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.DeleteAsync(deleteUri).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
