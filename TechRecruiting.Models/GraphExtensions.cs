using Microsoft.Azure.Graphs.Elements;
using System.Collections.Generic;
using System.Reflection;

namespace TechRecruiting.Models
{
    public static class GraphExtensions
    {
        public static List<T> ToStrongType<T>(this IList<Vertex> vertices) where T : IEntity, new()
        {
            List<T> results = new List<T>();

            foreach (Vertex vertex in vertices)
            {
                T result = new T
                {
                    Id = vertex.Id.ToString()
                };

                foreach (VertexProperty property in vertex.GetVertexProperties())
                {
                    PropertyInfo propertyInfo = typeof(T).GetProperty(property.Key);
                    propertyInfo.SetValue(result, property.Value);
                }

                results.Add(result);
            }

            return results;
        }
    }
}