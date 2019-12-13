using DFC.Logger.AppInsights.Contracts;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public class JobProfileSegmentRefreshService<TModel> : IJobProfileSegmentRefreshService<TModel>
    {
        private readonly ITopicClient topicClient;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public JobProfileSegmentRefreshService(ITopicClient topicClient, ICorrelationIdProvider correlationIdProvider)
        {
            this.topicClient = topicClient;
            this.correlationIdProvider = correlationIdProvider;
        }

        public async Task SendMessageAsync(TModel model)
        {
            var messageJson = JsonConvert.SerializeObject(model);
            var message = new Message(Encoding.UTF8.GetBytes(messageJson));
            message.CorrelationId = correlationIdProvider.CorrelationId;

            await topicClient.SendAsync(message).ConfigureAwait(false);
        }
    }
}