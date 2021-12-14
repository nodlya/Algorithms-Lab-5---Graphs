using System;
using System.IO;

namespace Graphs
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Graph graph = Graph.FromCSV(File.ReadAllLines(@"test2.csv"), '\t');
            GraphVisualizationData data = new GraphVisualizationData(graph);
            data.HighlightedEdges = new EdgeData[] { data.RawGraph.Edges[2] };
            data.HighlightedVertexes = new Vertex[] { data.HighlightedEdges[0].ToVertex, data.HighlightedEdges[0].FromVertex};
            Visualizer.CreateBitmaps(data);
            //graph.SaveCSV(@"graph1.csv");
            //graph.SaveJSON(@"graph1.json");
        }
    }
}
