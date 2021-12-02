using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Graphs
{
    public static class Visualizer
    {
        private static MagickImageCollection _collection = new MagickImageCollection();
        private static VisualizationConfiguration _configuration;

        public static void AddFrame(GraphVisualizationData data)
        {

        }

        public static void Visualize(this GraphVisualizationData data)
        {

        }

        private static void Magick()
        {
            using (_collection)
            {
                // Add first image and set the animation delay to 100ms
                _collection.Add("Snakeware.png");
                _collection[0].AnimationDelay = 100;

                // Add second image, set the animation delay to 100ms and flip the image
                _collection.Add("Snakeware.png");
                _collection[1].AnimationDelay = 100;
                _collection[1].Flip();

                // Optionally reduce colors
                QuantizeSettings settings = new QuantizeSettings();
                settings.Colors = 256;
                _collection.Quantize(settings);

                // Optionally optimize the images (images should have the same size).
                _collection.Optimize();

                // Save gif
                _collection.Write("Snakeware.Animated.gif");
            }
        }

        public static void Configure(VisualizationConfiguration config) => _configuration = config;

        public class VisualizationConfiguration
        {
            public readonly string PathToTempFile;
            public readonly string PathToGif;
        }
    }
}
