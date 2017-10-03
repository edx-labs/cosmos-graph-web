using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.Graphs.Elements;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechRecruiting.Models;

namespace TechRecruiting.Web.Data
{
    public sealed class CandidateData : GraphData
    {
        public async Task<IList<Candidate>> GetCandidates()
        {
            DocumentClient client = GetDocumentClient();
            DocumentCollection collection = await GetDocumentCollectionAsync(client);

            IDocumentQuery<Vertex> query = client.CreateGremlinQuery<Vertex>(
                collection,
                $"g.V().hasLabel('candidate')"
            );

            List<Vertex> vertices = new List<Vertex>();
            while (query.HasMoreResults)
            {
                vertices.AddRange(await query.ExecuteNextAsync<Vertex>());
            }

            return vertices.Select(vertex =>
                {
                    IEnumerable<VertexProperty> props = vertex.GetVertexProperties();
                    return new Candidate
                    {
                        Id = vertex.Id.ToString(),
                        FirstName = props.SingleOrDefault(p => p.Key == "firstName")?.Value?.ToString(),
                        LastName = props.SingleOrDefault(p => p.Key == "lastName")?.Value?.ToString(),
                        SkillDescription = props.SingleOrDefault(p => p.Key == "skillDescription")?.Value?.ToString()
                    };
                }
            ).ToList<Candidate>();
        }

        public async Task<Candidate> GetCandidateWithFriendships(string candidateId)
        {
            DocumentClient client = GetDocumentClient();
            DocumentCollection collection = await GetDocumentCollectionAsync(client);

            IDocumentQuery<Vertex> candidateQuery = client.CreateGremlinQuery<Vertex>(
                collection,
                $"g.V('{candidateId}').hasLabel('candidate')"
            );

            Vertex vertex = null;
            while (candidateQuery.HasMoreResults)
            {
                vertex = (await candidateQuery.ExecuteNextAsync<Vertex>()).First();
            }

            IEnumerable<VertexProperty> props = vertex.GetVertexProperties();
            Candidate candidate = new Candidate
            {
                Id = vertex.Id.ToString(),
                FirstName = props.SingleOrDefault(p => p.Key == "firstName")?.Value?.ToString(),
                LastName = props.SingleOrDefault(p => p.Key == "lastName")?.Value?.ToString(),
                SkillDescription = props.SingleOrDefault(p => p.Key == "skillDescription")?.Value?.ToString(),
                Friends = new List<Candidate>()
            };

            IDocumentQuery<Vertex> friendsQuery = client.CreateGremlinQuery<Vertex>(
                collection,
                $"g.V('{candidateId}').hasLabel('candidate').outE('acquaintance').inV().hasLabel('candidate')"
            );

            List<Vertex> vertices = new List<Vertex>();
            while (friendsQuery.HasMoreResults)
            {
                vertices.AddRange(await friendsQuery.ExecuteNextAsync<Vertex>());
            }

            foreach(var item in vertices)
            {
                IEnumerable<VertexProperty> itemProps = item.GetVertexProperties();
                candidate.Friends.Add(new Candidate
                    {
                        Id = item.Id.ToString(),
                        FirstName = itemProps.SingleOrDefault(p => p.Key == "firstName")?.Value?.ToString(),
                        LastName = itemProps.SingleOrDefault(p => p.Key == "lastName")?.Value?.ToString(),
                        SkillDescription = itemProps.SingleOrDefault(p => p.Key == "skillDescription")?.Value?.ToString()
                    }
                );
            }

            return candidate;
        }

        public async Task<Candidate> PersistCandidate(Candidate candidate)
        {
            DocumentClient client = GetDocumentClient();
            DocumentCollection collection = await GetDocumentCollectionAsync(client);

            IDocumentQuery<Vertex> query = client.CreateGremlinQuery<Vertex>(
                collection,
                $"g.addV('candidate').property('id', '{candidate.Id}').property('firstName', '{candidate.FirstName}').property('lastName', '{candidate.LastName}').property('skillDescription', '{candidate.SkillDescription}')"
            );

            Vertex vertex = null;
            while (query.HasMoreResults)
            {
                vertex = (await query.ExecuteNextAsync<Vertex>()).First();
            }

            candidate.Id = vertex.Id.ToString();

            return candidate;
        }
    }
}