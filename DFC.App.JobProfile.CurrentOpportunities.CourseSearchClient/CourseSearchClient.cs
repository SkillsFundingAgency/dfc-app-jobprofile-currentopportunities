using DFC.App.FindACourseClient.Contracts;
using DFC.App.FindACourseClient.Models;
using DFC.App.FindACourseClient.Models.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Extensions.Logging;
using System.Linq;
using DFC.App.FindACourseClient.Models.CosmosDb;
using DFC.App.FindACourseClient.Contracts.CosmosDb;
using Microsoft.Azure.Documents.Client;
using DFC.App.FindACourseClient.Repository.CosmosDb;
using System;

namespace DFC.App.FindACourseClient
{
    public class CourseSearchClient : ICourseSearchClient
    {
        private readonly ILogger<CourseSearchClient> logger;
        private readonly CourseSearchClientSettings courseSearchClientSettings;
        private readonly ICosmosRepository<APIAuditRecordCourse> auditRepository;

        public CourseSearchClient(CourseSearchClientSettings courseSearchClientSettings, ILogger<CourseSearchClient> logger = null, ICosmosRepository<APIAuditRecordCourse> auditRepository = null)
        {
            this.logger = logger;
            this.courseSearchClientSettings = courseSearchClientSettings;
            this.auditRepository = auditRepository;
        }

        public async Task<IEnumerable<CourseSumary>> GetCoursesAsync(string courseSearchKeywords)
        {
            logger?.LogInformation($"{nameof(GetCoursesAsync)} has been called with keywords {courseSearchKeywords} ");
            var request = MessageConverter.GetCourseListRequest(courseSearchKeywords, courseSearchClientSettings.courseSearchSvcSettings);


            var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

            //Used to initialize the OperationTimeout, which governs the whole process of sending a message, including receiving a reply message for a request/ reply service operation.
            binding.SendTimeout = new TimeSpan(0, 0, 0, courseSearchClientSettings.courseSearchSvcSettings.RequestTimeOutSeconds);

            var endpoint = new EndpointAddress(courseSearchClientSettings.courseSearchSvcSettings.ServiceEndpoint);
            var factory = new ChannelFactory<ServiceInterface>(binding, endpoint);
            var serviceInterfaceClient = (IClientChannel)factory.CreateChannel();

            bool success = false;
            try
            {
                var courseListResult = await ((ServiceInterface)serviceInterfaceClient).CourseListAsync(request);
                serviceInterfaceClient.Close();
                success = true;

                if (auditRepository != null)
                {
                    var auditRecord = new APIAuditRecordCourse() { DocumentId = Guid.NewGuid(), CorrelationId = Guid.NewGuid(), Request = request, Response = courseListResult };
                    await auditRepository.UpsertAsync(auditRecord).ConfigureAwait(false);
                }

                var convertedResults = courseListResult?.ConvertToCourse();
                logger?.LogInformation($"{nameof(GetCoursesAsync)} has returned {convertedResults.Count()} courses for keywords {courseSearchKeywords} ");
                return convertedResults;

            }
            finally
            {
                if (!success)
                {
                    serviceInterfaceClient.Abort();
                }
            }
        }
    }
}
