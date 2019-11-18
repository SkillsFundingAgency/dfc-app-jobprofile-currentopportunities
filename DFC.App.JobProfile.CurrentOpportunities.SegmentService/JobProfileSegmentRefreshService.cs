using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public class JobProfileSegmentRefreshService<TModel> : IJobProfileSegmentRefreshService<TModel>
    {
        private readonly ServiceBusOptions serviceBusOptions;

        public JobProfileSegmentRefreshService(ServiceBusOptions serviceBusOptions)
        {
            this.serviceBusOptions = serviceBusOptions;
        }

        public async Task SendMessageAsync(TModel model)
        {
            if (!string.IsNullOrWhiteSpace(serviceBusOptions.ServiceBusConnectionString) && !string.IsNullOrWhiteSpace(serviceBusOptions.TopicName))
            {
                var messageJson = JsonConvert.SerializeObject(model);
                var message = new Message(Encoding.UTF8.GetBytes(messageJson));
                var topicClient = new TopicClient(serviceBusOptions.ServiceBusConnectionString, serviceBusOptions.TopicName);

                await topicClient.SendAsync(message).ConfigureAwait(false);
            }
        }
    }
}