using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface ICourseCurrentOpportuntiesRefresh
    {
        Task<int> RefreshCoursesAsync(Guid documentId);

        //IEnumerable<Course> SelectCoursesForJobProfile(IEnumerable<Course> courses);
    }
}