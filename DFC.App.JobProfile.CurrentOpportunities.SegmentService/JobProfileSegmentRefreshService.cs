using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public class JobProfileSegmentRefreshService<TModel> : IJobProfileSegmentRefreshService<TModel>
    {
        private readonly ITopicClient topicClient;

        public JobProfileSegmentRefreshService(ITopicClient topicClient)
        {
            this.topicClient = topicClient;
        }

        public async Task SendMessageAsync(TModel model)
        {
            var messageJson = JsonConvert.SerializeObject(model);
            var message = new Message(Encoding.UTF8.GetBytes(messageJson));

            await topicClient.SendAsync(message).ConfigureAwait(false);
        }
    }
}