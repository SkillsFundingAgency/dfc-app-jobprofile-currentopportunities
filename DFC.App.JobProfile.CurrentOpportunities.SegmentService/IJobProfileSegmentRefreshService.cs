using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public interface IJobProfileSegmentRefreshService<in TModel>
    {
        Task SendMessageAsync(TModel model);
    }
}