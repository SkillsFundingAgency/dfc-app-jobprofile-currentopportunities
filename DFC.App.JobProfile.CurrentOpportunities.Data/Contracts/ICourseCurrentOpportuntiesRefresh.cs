using DFC.FindACourseClient.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface ICourseCurrentOpportuntiesRefresh
    {
        Task<int> RefreshCoursesAsync(Guid documentId);

        IEnumerable<CourseSumary> SelectCoursesForJobProfile(IEnumerable<CourseSumary> courses);
    }
}
