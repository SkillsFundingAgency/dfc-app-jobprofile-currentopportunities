using DFC.App.FindACourseClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.FindACourseClient.Contracts
{
    public interface ICourseSearchClient
    {
        Task<IEnumerable<CourseSumary>> GetCoursesAsync(string jobProfileKeywords);
    }
}
