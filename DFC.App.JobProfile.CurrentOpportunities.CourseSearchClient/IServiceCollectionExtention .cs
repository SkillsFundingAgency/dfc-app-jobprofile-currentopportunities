using DFC.App.FindACourseClient.Contracts.CosmosDb;
using DFC.App.FindACourseClient.Models.CosmosDb;
using DFC.App.FindACourseClient.Repository.CosmosDb;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.DependencyInjection;


namespace DFC.App.FindACourseClient.Models.Configuration
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddFindACourseServices(this IServiceCollection services, CourseSearchClientSettings courseSearchClientSettings)
        {
            services.AddSingleton<ICosmosRepository<APIAuditRecordCourse>, CosmosRepository<APIAuditRecordCourse>>(s =>
            {
                var cosmosDbAuditConnection = courseSearchClientSettings.courseSearchAuditCosmosDbSettings;
                var documentClient = new DocumentClient(cosmosDbAuditConnection.EndpointUrl, cosmosDbAuditConnection.AccessKey);
                return new CosmosRepository<APIAuditRecordCourse>(cosmosDbAuditConnection, documentClient);
            });
            return services;
        }
    }
}
