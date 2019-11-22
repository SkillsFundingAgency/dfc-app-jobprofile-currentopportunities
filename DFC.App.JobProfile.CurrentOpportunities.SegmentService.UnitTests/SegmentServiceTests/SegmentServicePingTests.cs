using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using DFC.App.JobProfile.CurrentOpportunities.SegmentService;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Ping / Health Tests")]
    public class SegmentServicePingTests : SegmentServiceBaseTests
    {
        public SegmentServicePingTests() : base()
        {
        }

        [Fact]
        public void CareerPathSegmentServicePingReturnsSuccess()
        {
            // arrange
            var expectedResult = true;
            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            // act
            var result = currentOpportunitiesSegmentService.PingAsync().Result;

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void CareerPathSegmentServicePingReturnsFalseWhenMissingRepository()
        {
            // arrange
            var expectedResult = false;

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            // act
            var result = currentOpportunitiesSegmentService.PingAsync().Result;

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}