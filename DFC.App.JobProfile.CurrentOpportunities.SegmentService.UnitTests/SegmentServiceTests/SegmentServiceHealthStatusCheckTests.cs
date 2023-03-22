using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Health Status Tests")]
    public class SegmentServiceHealthStatusCheckTests
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> fakeRepository;
        private readonly ICourseCurrentOpportunitiesRefresh fakeCourseCurrentOpportunitiesRefresh;
        private readonly IAVCurrentOpportunitiesRefresh fakeIAVCurrentOpportunatiesRefresh;
        private readonly ILogger<CurrentOpportunitiesSegmentService> fakeLogger;
        private readonly IMapper fakeMapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> fakeJobProfileSegmentRefreshService;
        private readonly ICurrentOpportunitiesSegmentUtilities fakeCurrentOpportunitiesSegmentUtilities;

        public SegmentServiceHealthStatusCheckTests()
        {
            fakeRepository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            fakeCourseCurrentOpportunitiesRefresh = A.Fake<ICourseCurrentOpportunitiesRefresh>();
            fakeIAVCurrentOpportunatiesRefresh = A.Fake<IAVCurrentOpportunitiesRefresh>();
            fakeLogger = A.Fake<ILogger<CurrentOpportunitiesSegmentService>>();
            fakeMapper = A.Fake<IMapper>();
            fakeJobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            fakeCurrentOpportunitiesSegmentUtilities = A.Fake<ICurrentOpportunitiesSegmentUtilities>();
        }

        [Theory]
        [InlineData(true, HealthStatus.Healthy)]
        [InlineData(false, HealthStatus.Degraded)]
        public async Task GetCurrentHealthStatusAsyncTestAsync(bool isHealthyResponse, HealthStatus expectedStatus)
        {
            // arrange
            var dummyHealthCheckContext = A.Dummy<HealthCheckContext>();
            A.CallTo(() => fakeRepository.PingAsync()).Returns(isHealthyResponse);
            var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(fakeRepository, fakeCourseCurrentOpportunitiesRefresh, fakeIAVCurrentOpportunatiesRefresh, fakeLogger, fakeMapper, fakeJobProfileSegmentRefreshService, fakeCurrentOpportunitiesSegmentUtilities);

            //Act
            var serviceHealthStatus = await currentOpportunitiesSegmentService.CheckHealthAsync(dummyHealthCheckContext).ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.Status.Should().Be(expectedStatus);
            A.CallTo(() => fakeRepository.PingAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void GetCurrentHealthStatusAsyncExceptionTestAsync()
        {
            //Arrange
            var repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            var dummyHealthCheckContext = A.Dummy<HealthCheckContext>();
            A.CallTo(() => repository.PingAsync()).Throws(new ApplicationException());
            var currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, fakeCourseCurrentOpportunitiesRefresh, fakeIAVCurrentOpportunatiesRefresh, fakeLogger, fakeMapper, fakeJobProfileSegmentRefreshService, fakeCurrentOpportunitiesSegmentUtilities);

            //Act
            Func<Task> serviceHealthStatus = async () => await currentOpportunitiesSegmentService.CheckHealthAsync(dummyHealthCheckContext).ConfigureAwait(false);

            //Asserts
            serviceHealthStatus.Should().ThrowAsync<Exception>();
        }
    }
}