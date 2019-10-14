using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{

    [Trait("Segment Service", "Health Status Tests")]
    public class SegmentServiceHealthStatusCheckTests
    {

        [Fact]
        public async Task GetCurrentHealthStatusAsyncTestAsync()
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            var expectedResult = true;
            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);
            var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, A.Fake<IDraftCurrentOpportunitiesSegmentService>());

            //Act
            var serviceHealthStatus = await currentOpportunitiesSegmentService.GetCurrentHealthStatusAsync().ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.HealthServiceState.Should().Be(HealthServiceState.Green);
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetCurrentHealthStatusAsyncExceptionTestAsync()
        {
            //Arrange
            var repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            A.CallTo(() => repository.PingAsync()).Throws(new ApplicationException());
            var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, A.Fake<IDraftCurrentOpportunitiesSegmentService>());

            //Act
            var serviceHealthStatus = await currentOpportunitiesSegmentService.GetCurrentHealthStatusAsync().ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.HealthServiceState.Should().Be(HealthServiceState.Red);
        }
    }
}
