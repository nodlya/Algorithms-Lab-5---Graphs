using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Graphs
{
    /// <summary>
    /// Represents a graph.
    /// </summary>
    public class Graph
    {
        public List<Vertex> Vertexes { get; set; }
        public List<EdgeData> Edges { get; set; }

        public static Graph FromJSON(string json)
        {
            var graph = JsonSerializer.Deserialize<Graph>(json, JsonOptions);
            graph.Restore();
            return graph;
        }
        public static Graph FromCSV(string[] csv, char separator = ';')
        {
            var graph = new Graph();

            string[][] textMatrix = csv.Select(r => r.Split(separator)).ToArray();

            int Vertexes = textMatrix.Length;

            int temp = 0;
            graph.Vertexes = new Vertex[Vertexes].Select(p => new Vertex() { Number = temp++ }).ToList();

            int?[][] matrix = new int?[Vertexes][].Select(r => new int?[Vertexes]).ToArray();

            for (int y = 0; y < Vertexes; y++)
            {
                for (int x = 0; x < Vertexes; x++)
                {
                    int result;
                    if (int.TryParse(textMatrix[y][x], out result)) matrix[y][x] = result;
                }
            }

            List<EdgeData> edges = new List<EdgeData>();

            for (int y = 0; y < Vertexes; y++)
            {
                for (int x = 0; x < Vertexes; x++)
                {
                    if (matrix[y][x].HasValue) edges.Add(new EdgeData(y, matrix[y][x].Value, x));
                }
            }

            graph.Edges = edges.ToList();
            graph.Restore();

            return graph;
        }

        public void SaveJSON(string path) => File.WriteAllText(path, JsonSerializer.Serialize(this, JsonOptions));
        public void SaveCSV(string path, char separator = ';')
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < Vertexes.Count; y++)
            {
                for (int x = 0; x < Vertexes.Count; x++)
                {
                    EdgeData edge = Edges.Find(e => e.From == y && e.To == x);
                    if (edge != null) sb.Append(edge.Weight);
                    sb.Append(separator);
                }
                sb.Append('\n');
            }
            File.WriteAllText(path, sb.ToString());
        }

        private void Restore()
        {
            foreach (var ed in Edges)
            {
                var from = Vertexes.Find(v => v.Number == ed.From);
                var to = Vertexes.Find(v => v.Number == ed.To);

                from.Edges.Add(new Edge()
                {
                    Weight = ed.Weight,
                    Destination = ed.To,
                    DestinationVertex = to
                });
            }
        }

        private readonly static JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
    }

    public class Vertex
    {
        public int Number { get; set; }
        public List<Edge> Edges { get; set; } = new List<Edge>();
    }


    public class Edge
    {
        public int Destination { get; set; }
        public int Weight { get; set; }      

        [JsonIgnore()]
        public Vertex DestinationVertex { get; set; }
    }

    /// <summary>
    /// Represents graph's edge containing both of the points.
    /// </summary>
    public class EdgeData
    {
        /// <summary>
        /// Represents the first position vertex as a Vertex object. This is not JSON-serializable.
        /// </summary>
        [JsonIgnore]
        public Vertex FromVertex { get; set; }

        /// <summary>
        /// Represents the first position vertex as Int32.
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// Represents a weight of this Edge. 
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Represents the second position vertex as a Vertex object. This is not JSON-serializable.
        /// </summary>
        [JsonIgnore]
        public Vertex ToVertex { get; set; }

        /// <summary>
        /// Represents the second position vertex as Int32.
        /// </summary>
        public int To { get; set; }

        public EdgeData(int from, int weight, int to)
        {
            From = from;
            Weight = weight;
            To = to;
        }
    }

    public class GraphVisualizationData
    {
        public readonly Graph RawGraph;
        public Vertex[] HighlightedVertexes { get; set; }
        public EdgeData[] HighlightedEdges { get; set; }
      
        public GraphVisualizationData(Graph rawGraph)
        {
            RawGraph = rawGraph;
        }
    }
}
