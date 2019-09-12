using System;
using System.Collections.Generic;
using System.Net;
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
        }

        public static IEnumerable<object[]> SegmentContentRouteData => new List<object[]>
        {
            new object[] { "/Segment", "text/html" },
            new object[] { $"/Segment/{DefaultArticleName}" , "text/html"},
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
    }
}