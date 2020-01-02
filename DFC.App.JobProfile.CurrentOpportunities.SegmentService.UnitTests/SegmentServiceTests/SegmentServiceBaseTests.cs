using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServiceBaseTests
    {
        public SegmentServiceBaseTests()
        {
            FakeRepository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            FakeCourseCurrentOpportunitiesRefresh = A.Fake<ICourseCurrentOpportunitiesRefresh>();
            FakeAVCurrentOpportunatiesRefresh = A.Fake<IAVCurrentOpportunitiesRefresh>();
            FakeLogger = A.Fake<ILogger<CurrentOpportunitiesSegmentService>>();
            FakeMapper = A.Fake<IMapper>();
            FakeJobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            FakeCurrentOpportunitiesSegmentUtilities = A.Fake<ICurrentOpportunitiesSegmentUtilities>();

            CurrentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(FakeRepository, FakeCourseCurrentOpportunitiesRefresh, FakeAVCurrentOpportunatiesRefresh, FakeLogger, FakeMapper, FakeJobProfileSegmentRefreshService, FakeCurrentOpportunitiesSegmentUtilities);
        }

        protected ICosmosRepository<CurrentOpportunitiesSegmentModel> FakeRepository { get; }

        protected ICourseCurrentOpportunitiesRefresh FakeCourseCurrentOpportunitiesRefresh { get; }

        protected IAVCurrentOpportunitiesRefresh FakeAVCurrentOpportunatiesRefresh { get; }

        protected ILogger<CurrentOpportunitiesSegmentService> FakeLogger { get; }

        protected IMapper FakeMapper { get; }

        protected IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> FakeJobProfileSegmentRefreshService { get; }

        protected ICurrentOpportunitiesSegmentUtilities FakeCurrentOpportunitiesSegmentUtilities { get; }

        protected ICurrentOpportunitiesSegmentService CurrentOpportunitiesSegmentService { get; set; }
    }
}