using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.Graphs.Elements;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechRecruiting.Web.Data
{
    public sealed class ReportData : GraphData
    {
        public async Task<ILookup<string, string>> GetPoachers(string recruiterId)
        {
            DocumentClient client = GetDocumentClient();
            DocumentCollection collection = await GetDocumentCollectionAsync(client);

            IDocumentQuery<Vertex> query = client.CreateGremlinQuery<Vertex>(
                collection,
                $"g.V('{recruiterId}').outE('acquaintance').inV().inE('likes')"
            );

            List<Edge> edges = new List<Edge>();
            while (query.HasMoreResults)
            {
                edges.AddRange(await query.ExecuteNextAsync<Edge>());
            }
            
            return edges.ToLookup(e => e.OutVertexId.ToString(), e => e.InVertexId.ToString());
        }
    }
}