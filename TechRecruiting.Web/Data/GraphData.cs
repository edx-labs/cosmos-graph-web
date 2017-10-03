using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace TechRecruiting.Web.Data
{
    public abstract class GraphData
    {

        private readonly string _endpointUrl = ConfigurationManager.AppSettings["CosmosEndpointUrl"];
        private readonly string _accountKey = ConfigurationManager.AppSettings["CosmosAccountKey"];
        protected readonly string _databaseName = ConfigurationManager.AppSettings["CosmosDatabaseName"];
        protected readonly string _graphName = ConfigurationManager.AppSettings["CosmosGraphName"];

        protected async Task<DocumentCollection> GetDocumentCollectionAsync(DocumentClient client)
        {
            Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseName });
            DocumentCollection collection = await client.CreateDocumentCollectionIfNotExistsAsync(database.SelfLink, new DocumentCollection { Id = _graphName }, new RequestOptions { OfferThroughput = 400 });
            return collection;
        }

        protected DocumentClient GetDocumentClient()
        {
             return new DocumentClient(new Uri(_endpointUrl), _accountKey);
        }
    }
}