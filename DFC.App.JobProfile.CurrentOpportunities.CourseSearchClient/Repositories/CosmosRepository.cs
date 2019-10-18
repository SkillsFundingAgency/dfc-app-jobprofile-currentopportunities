using DFC.App.FindACourseClient.Contracts;
using DFC.App.FindACourseClient.Contracts.CosmosDb;
using DFC.App.FindACourseClient.Models.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.FindACourseClient.Repository.CosmosDb
{
    public class CosmosRepository<T> : ICosmosRepository<T>
    where T : IDataModel
    {
        private readonly CourseSearchAuditCosmosDbSettings cosmosDbConnection;
        private readonly IDocumentClient documentClient;

        public CosmosRepository(CourseSearchAuditCosmosDbSettings cosmosDbConnection, IDocumentClient documentClient)
        {
            this.cosmosDbConnection = cosmosDbConnection;
            this.documentClient = documentClient;
        }

        private Uri DocumentCollectionUri => UriFactory.CreateDocumentCollectionUri(cosmosDbConnection.DatabaseId, cosmosDbConnection.CollectionId);

   
        public async Task<HttpStatusCode> UpsertAsync(T model)
        {
            var ac = new AccessCondition { Condition = model.Etag, Type = AccessConditionType.IfMatch };
            var pk = new PartitionKey(model.PartitionKey);

            var result = await documentClient.UpsertDocumentAsync(DocumentCollectionUri, model, new RequestOptions { AccessCondition = ac, PartitionKey = pk }).ConfigureAwait(false);

            return result.StatusCode;
        }

        private Uri CreateDocumentUri(Guid documentId)
        {
            return UriFactory.CreateDocumentUri(cosmosDbConnection.DatabaseId, cosmosDbConnection.CollectionId, documentId.ToString());
        }
    }
}