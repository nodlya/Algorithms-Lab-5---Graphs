using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Graphs
{
    public static class Visualizer
    {
        static int windowSize = 1024;
        static int padding = windowSize/5;
        static int center = windowSize/2;
        static double radius = 0.3*windowSize;
        static float nodeRadius = 20;
        static GraphVisualizationData graph;
        static Point[] placeVertexes;
        static SolidBrush basicBrush = new SolidBrush(Color.Black);
        static SolidBrush highlightBrush = new SolidBrush(Color.Red);
        static SolidBrush backgroundColor = new SolidBrush(Color.White);
        static SolidBrush sourseBrush = new SolidBrush(Color.Orange);
        static SolidBrush drainBrush = new SolidBrush(Color.Blue);
        static Pen basicPen = new Pen(Color.Black);
        static Pen highlightedPen = new Pen(Color.Red);
        static Pen soursePen = new Pen(Color.Orange);
        static Pen drainPen = new Pen(Color.LightBlue);
        public static void CreateBitmaps(GraphVisualizationData graphData)
        {
            Bitmap picture = new Bitmap(windowSize, windowSize);
            graph = graphData;
            GetVertexPoints();
            DrawVertex(picture);
        }

        static void DrawVertex(Bitmap picture)
        {
            Graphics g = Graphics.FromImage(picture);
            g.FillRectangle(backgroundColor,0,0,windowSize,windowSize);
            DrawEdges(g);
            for (int i = 0; i<placeVertexes.Length; i++)
            {
                Pen pen = basicPen;
                Brush brush = backgroundColor;
                try
                {
                    pen = graph.HighlightedVertexes.Contains(graph.RawGraph.Vertexes[i]) ? highlightedPen : basicPen;
                    brush = graph.HighlightedEdges.Contains(graph.RawGraph.Edges[i]) ? highlightBrush : backgroundColor;
                }
                catch
                {

                }
                if (i == 0) 
                { 
                    pen = soursePen;
                    brush = sourseBrush;
                }
                if (i == placeVertexes.Length - 1)
                {
                    pen = drainPen;
                    brush = drainBrush;
                }
                g.DrawEllipse(pen, placeVertexes[i].X - nodeRadius, placeVertexes[i].Y - nodeRadius,
                                    nodeRadius + nodeRadius, nodeRadius + nodeRadius);
                g.FillEllipse(brush, placeVertexes[i].X - nodeRadius, placeVertexes[i].Y - nodeRadius,
                                 nodeRadius + nodeRadius, nodeRadius + nodeRadius);
                g.DrawString(Convert.ToString(i + 1), new Font("Arial", 20), 
                                    basicBrush, placeVertexes[i].X - 10, placeVertexes[i].Y - 13);
            }
            WriteLogs(g);
            SavePicture(picture);
        }

        static Graphics DrawEdges(Graphics picture)
        {
            for (int i = 0;i < graph.RawGraph.Edges.Count;i++)
            {
                var t1 = graph.RawGraph.Edges[i].From;
                var t2 = graph.RawGraph.Edges[i].To;
                int x1 = 0;
                int x2 = 0;
                int y1 = 0;
                int y2 = 0;
               
                x1 = (int)(placeVertexes[t1].X);
                x2 = (int)(placeVertexes[t2].X);
                y1 = (int)(placeVertexes[t1].Y);
                y2 = (int)(placeVertexes[t2].Y);
                                
                double sin = (x1*y1+x2*y2)/(Math.Sqrt(x1*x1+x2*x2)*Math.Sqrt(y1*y1+y2*y2));
                double cos = Math.Sqrt(1-sin*sin);
                if (y1 < y2)
                {
                    x2 += (int)(cos * nodeRadius);
                    y2 -= (int)(sin * nodeRadius);
                }
                else if (y1==y2) x2 -= (int)nodeRadius;
                else
                {
                    x2 -= (int)(cos * nodeRadius);
                    y2 += (int)(sin * nodeRadius);
                }
                Pen pen = basicPen;
                Brush brush = basicBrush;
                try
                {
                    pen = graph.HighlightedVertexes.Contains(graph.RawGraph.Vertexes[i]) ? highlightedPen : basicPen;
                    brush = graph.HighlightedEdges.Contains(graph.RawGraph.Edges[i]) ?  highlightBrush : basicBrush;
                }
                catch
                {

                }
                

                pen.CustomEndCap = new AdjustableArrowCap(6, 6);
                picture.RotateTransform((float)Math.Asin(sin));
                picture.DrawString(Convert.ToString(graph.RawGraph.Edges[i].Weight),
                    new Font(FontFamily.GenericMonospace,20), brush,(x2+x1)/2,(y2+y1)/2);
                picture.RotateTransform(-((float)Math.Asin(sin)));
                picture.DrawLine(pen,new Point(x1,y1),new Point(x2,y2));
               
            }
            return picture;
        }

        static void WriteLogs(Graphics g)
        {
            string textLog = "sdffd"; //напишите лог
            int x = center;
            int y = (int)(0.9 * windowSize);
            g.DrawString(textLog, new Font(FontFamily.GenericMonospace, 25), new SolidBrush(Color.Black), x, y);
        }

        static void GetVertexPoints()
        {
            int length = graph.RawGraph.Vertexes.Count;
            placeVertexes = new Point[length];
            placeVertexes[length - 1] = new Point(windowSize - padding, center);
            int vertexCount = length;
            double angle = Math.PI / (vertexCount-1);
            for (int i = 0; i < length; i++)
            {
                placeVertexes[i] = new Point((int)(center-radius*Math.Cos(angle*i)), (int)(center - radius * Math.Sin(angle * i) * Math.Pow(-1,i)));
            }    
        }

        static void SavePicture(Bitmap picture,int picNumber = 0)
        {

            //введите своё название + picNumber
            picture.Save(@"test.png");
        }

        public static void SaveGIF(int num)
        {
            using (MagickImageCollection collection = new MagickImageCollection())
            {
                string path = @""; //напишите название картинок
                // Add first image and set the animation delay to 100ms
                for (int i = 2; i <= num; i++)
                {
                    collection.Add(path + i + ".png");
                    collection[i - 2].AnimationDelay = 100;
                }

                // Optionally reduce colors
                QuantizeSettings settings = new QuantizeSettings();
                settings.Colors = 256;
                collection.Quantize(settings);

                // Optionally optimize the images (images should have the same size).
                collection.Optimize();


                // Save gif
                collection.Write("result.gif");
            }

        }
    }
}
