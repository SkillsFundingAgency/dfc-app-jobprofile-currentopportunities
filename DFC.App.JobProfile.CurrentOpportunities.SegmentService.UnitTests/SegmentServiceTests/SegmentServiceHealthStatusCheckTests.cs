using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Health Status Tests")]
    public class SegmentServiceHealthStatusCheckTests
    {
        [Theory]
        [InlineData(true, HealthStatus.Healthy)]
        [InlineData(false, HealthStatus.Degraded)]
        public async Task GetCurrentHealthStatusAsyncTestAsync(bool isHealthyResponse, HealthStatus expectedStatus)
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            var dummyHealthCheckContext = A.Dummy<HealthCheckContext>();
            A.CallTo(() => repository.PingAsync()).Returns(isHealthyResponse);
            var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, A.Fake<IDraftCurrentOpportunitiesSegmentService>());

            //Act
            var serviceHealthStatus = await currentOpportunitiesSegmentService.CheckHealthAsync(dummyHealthCheckContext).ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.Status.Should().Be(expectedStatus);
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void GetCurrentHealthStatusAsyncExceptionTestAsync()
        {
            //Arrange
            var repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            var dummyHealthCheckContext = A.Dummy<HealthCheckContext>();
            A.CallTo(() => repository.PingAsync()).Throws(new ApplicationException());
            var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, A.Fake<IDraftCurrentOpportunitiesSegmentService>());

            //Act
            Func<Task> serviceHealthStatus = async () => await currentOpportunitiesSegmentService.CheckHealthAsync(dummyHealthCheckContext).ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.Should().Throw<Exception>();
        }
    }
}
