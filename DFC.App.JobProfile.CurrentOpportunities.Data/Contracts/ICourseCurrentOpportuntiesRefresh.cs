using DFC.App.FindACourseClient.Models;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface ICourseCurrentOpportuntiesRefresh
    {
        Task<int> RefreshCoursesAsync(Guid documentId);

        IEnumerable<CourseSumary> SelectCoursesForJobProfile(IEnumerable<CourseSumary> courses);
    }
}
