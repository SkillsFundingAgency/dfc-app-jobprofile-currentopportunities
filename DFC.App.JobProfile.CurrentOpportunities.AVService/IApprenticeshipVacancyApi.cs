using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService
{
    public interface IApprenticeshipVacancyApi
    {
        Task<string> GetAsync(string requestQueryString, RequestType requestTyp);
    }

    public enum RequestType
    {
        search,
        apprenticeships
    }
}