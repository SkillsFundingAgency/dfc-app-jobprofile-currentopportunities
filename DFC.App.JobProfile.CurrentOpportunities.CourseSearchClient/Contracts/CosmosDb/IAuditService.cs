using System.Threading.Tasks;

namespace DFC.App.FindACourseClient.Contracts.CosmosDb
{
    public interface IAuditService
    {
        Task AuditAsync(string request, string response);
    }
}
