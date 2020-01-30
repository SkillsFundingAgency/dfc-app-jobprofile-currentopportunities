using DFC.Logger.AppInsights.Contracts;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService
{
    public class JobProfileSegmentRefreshService<TModel> : IJobProfileSegmentRefreshService<TModel>
    {
        private readonly ITopicClient topicClient;
        private readonly ICorrelationIdProvider correlationIdProvider;
        private readonly ILogService logService;

        public JobProfileSegmentRefreshService(ITopicClient topicClient, ICorrelationIdProvider correlationIdProvider, ILogService logService)
        {
            this.topicClient = topicClient;
            this.correlationIdProvider = correlationIdProvider;
            this.logService = logService;
        }

        public async Task SendMessageAsync(TModel model)
        {
            try
            {
                var message = CreateMessage(model);
                await topicClient.SendAsync(message).ConfigureAwait(false);
            }
            catch (ServiceBusException e)
            {
                logService.LogWarning($"Unable to refresh JobProfile '{JsonConvert.SerializeObject(model)}'. Error: {e.Message}");
            }
        }

        public async Task SendMessageListAsync(IList<TModel> models)
        {
            if (models != null)
            {
                foreach (var model in models)
                {
                    await SendMessageAsync(model).ConfigureAwait(false);
                }
            }
        }

        private Message CreateMessage(TModel model)
        {
            var messageJson = JsonConvert.SerializeObject(model);
            return new Message(Encoding.UTF8.GetBytes(messageJson))
            {
                CorrelationId = correlationIdProvider.CorrelationId,
            };
        }
    }
}