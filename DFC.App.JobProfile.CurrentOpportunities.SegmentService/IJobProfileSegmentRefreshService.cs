using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public interface IJobProfileSegmentRefreshService<TModel>
    {
        Task SendMessageAsync(TModel model);

        Task SendMessageListAsync(IList<TModel> models);
    }
}