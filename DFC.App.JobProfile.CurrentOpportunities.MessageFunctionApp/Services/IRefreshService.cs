using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services
{
    public interface IRefreshService
    {
        Task<IList<SimpleJobProfileModel>> GetSimpleListAsync();

        Task<HttpStatusCode> RefreshApprenticeshipsAsync(Guid documentId);

        Task<HttpStatusCode> RefreshCoursesAsync(Guid documentId);
    }
}
