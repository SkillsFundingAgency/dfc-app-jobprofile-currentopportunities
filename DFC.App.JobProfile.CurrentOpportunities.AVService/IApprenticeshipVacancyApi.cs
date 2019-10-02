using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService
{
    public enum RequestType
    {
        Search,
        Apprenticeships,
    }

    public interface IApprenticeshipVacancyApi
    {
        Task<string> GetAsync(string requestQueryString, RequestType requestType);
    }
}