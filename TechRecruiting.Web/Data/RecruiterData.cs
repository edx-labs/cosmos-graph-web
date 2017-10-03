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
    public sealed class RecruiterData : GraphData
    {
        public async Task<IEnumerable<Recruiter>> GetRecruiters()
        {
            DocumentClient client = GetDocumentClient();
            DocumentCollection collection = await GetDocumentCollectionAsync(client);

            IDocumentQuery<Vertex> query = client.CreateGremlinQuery<Vertex>(
                collection,
                $"g.V().hasLabel('recruiter')"
            );

            List<Vertex> vertices = new List<Vertex>();
            while (query.HasMoreResults)
            {
                vertices.AddRange(await query.ExecuteNextAsync<Vertex>());
            }

            return vertices.Select(vertex =>
            {
                IEnumerable<VertexProperty> props = vertex.GetVertexProperties();
                return new Recruiter
                {
                    Id = vertex.Id.ToString(),
                    FirstName = props.SingleOrDefault(p => p.Key == "firstName")?.Value?.ToString(),
                    LastName = props.SingleOrDefault(p => p.Key == "lastName")?.Value?.ToString()
                };
            });
        }

        public async Task<Recruiter> GetRecruiterWithCandidates(string recruiterId)
        {
            DocumentClient client = GetDocumentClient();
            DocumentCollection collection = await GetDocumentCollectionAsync(client);

            IDocumentQuery<Vertex> candidateQuery = client.CreateGremlinQuery<Vertex>(
                collection,
                $"g.V('{recruiterId}').hasLabel('recruiter')"
            );

            Vertex vertex = null;
            while (candidateQuery.HasMoreResults)
            {
                vertex = (await candidateQuery.ExecuteNextAsync<Vertex>()).First();
            }

            IEnumerable<VertexProperty> props = vertex.GetVertexProperties();
            Recruiter candidate = new Recruiter
            {
                Id = vertex.Id.ToString(),
                FirstName = props.SingleOrDefault(p => p.Key == "firstName")?.Value?.ToString(),
                LastName = props.SingleOrDefault(p => p.Key == "lastName")?.Value?.ToString(),
                Candidates = new List<Candidate>()
            };

            IDocumentQuery<Vertex> friendsQuery = client.CreateGremlinQuery<Vertex>(
                collection,
                $"g.V('{recruiterId}').hasLabel('recruiter').outE('acquaintance').inV().hasLabel('candidate')"
            );

            List<Vertex> vertices = new List<Vertex>();
            while (friendsQuery.HasMoreResults)
            {
                vertices.AddRange(await friendsQuery.ExecuteNextAsync<Vertex>());
            }

            foreach (var item in vertices)
            {
                IEnumerable<VertexProperty> itemProps = item.GetVertexProperties();
                candidate.Candidates.Add(new Candidate
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

        public async Task<Recruiter> PersistRecruiter(Recruiter recruiter)
        {
            DocumentClient client = GetDocumentClient();
            DocumentCollection collection = await GetDocumentCollectionAsync(client);

            IDocumentQuery<Vertex> query = client.CreateGremlinQuery<Vertex>(
                collection,
                $"g.addV('candidate').property('id', '{recruiter.Id}').property('firstName', '{recruiter.FirstName}').property('lastName', '{recruiter.LastName}')"
            );

            Vertex vertex = null;
            while (query.HasMoreResults)
            {
                vertex = (await query.ExecuteNextAsync<Vertex>()).First();
            }

            recruiter.Id = vertex.Id.ToString();

            return recruiter;
        }
    }
}