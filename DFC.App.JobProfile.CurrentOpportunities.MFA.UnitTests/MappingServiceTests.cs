using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Functions;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Models;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests
{
    [Trait("MFA Tests", "Mapping Service Tests")]
    public class MappingServiceTests
    {

        [Fact]
        public void MapToSegmentModelTest()
        {
            //Arrange
            var fakeMapper = A.Fake<IMapper>();
            var mappingService = new MappingService(fakeMapper);
            var expectedSequenceNumber = 1234;
            var testCurrentOpportunitiesSegmentModel = new CurrentOpportunitiesSegmentModel()
            {
                Data = new CurrentOpportunitiesSegmentDataModel()
                {
                    Apprenticeships = new Apprenticeships(),
                    Courses = new Courses(),
                },
            };

            A.CallTo(() => fakeMapper.Map<CurrentOpportunitiesSegmentModel>(A<JobProfileMessage>.Ignored)).Returns(testCurrentOpportunitiesSegmentModel);
            A.CallTo(() => fakeMapper.Map<Apprenticeships>(A<JobProfileMessage>.Ignored)).Returns(A.Dummy<Apprenticeships>());
            A.CallTo(() => fakeMapper.Map<Courses>(A<JobProfileMessage>.Ignored)).Returns(A.Dummy<Courses>());
            var testSegmentJson = "{}";

            //Act
            var result = mappingService.MapToSegmentModel(testSegmentJson, expectedSequenceNumber);

            //Asserts
            result.SequenceNumber.Should().Be(expectedSequenceNumber);
            A.CallTo(() => fakeMapper.Map<CurrentOpportunitiesSegmentModel>(A<JobProfileMessage>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<Apprenticeships>(A<JobProfileMessage>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<Courses>(A<JobProfileMessage>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}
