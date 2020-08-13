using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface ICourseCurrentOpportunitiesRefresh
    {
        Task<int> RefreshCoursesAsync(Guid documentId);

        Task<int> RefreshCoursesAndUpdateJobProfileAsync(Guid documentId);
    }
}