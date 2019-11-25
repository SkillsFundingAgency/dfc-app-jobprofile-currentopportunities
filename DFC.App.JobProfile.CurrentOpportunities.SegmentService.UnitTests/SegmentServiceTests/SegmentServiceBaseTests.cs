using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;


namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServiceBaseTests
    {

        protected readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
       
        protected ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService { get; set; }
        protected readonly ICourseCurrentOpportuntiesRefresh fakeCourseCurrentOpportuntiesRefresh;
        protected readonly IAVCurrentOpportuntiesRefresh fakeAVCurrentOpportunatiesRefresh;
        protected readonly ILogger<CurrentOpportunitiesSegmentService> fakeLogger;
        protected readonly IMapper fakeMapper;
        protected readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> fakeJobProfileSegmentRefreshService;
        protected readonly ICurrentOpportunitiesSegmentUtilities fakeCurrentOpportunitiesSegmentUtilities;

        public SegmentServiceBaseTests()
        {
            repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            fakeCourseCurrentOpportuntiesRefresh = A.Fake<ICourseCurrentOpportuntiesRefresh>();
            fakeAVCurrentOpportunatiesRefresh = A.Fake<IAVCurrentOpportuntiesRefresh>();
            fakeLogger = A.Fake<ILogger<CurrentOpportunitiesSegmentService>>();
            fakeMapper = A.Fake<IMapper>();
            fakeJobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            fakeCurrentOpportunitiesSegmentUtilities = A.Fake<ICurrentOpportunitiesSegmentUtilities>();

            currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, fakeCourseCurrentOpportuntiesRefresh, fakeAVCurrentOpportunatiesRefresh, fakeLogger, fakeMapper, fakeJobProfileSegmentRefreshService, fakeCurrentOpportunitiesSegmentUtilities);
        }

    }
}
