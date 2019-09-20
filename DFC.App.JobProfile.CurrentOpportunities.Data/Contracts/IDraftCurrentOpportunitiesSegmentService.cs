using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Contracts
{
    public interface IDraftCurrentOpportunitiesSegmentService
    {
        Task<CurrentOpportunitiesSegmentModel> GetSitefinityData(string canonicalName);
    }
}
