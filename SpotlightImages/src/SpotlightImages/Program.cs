using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotlightImages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var spotlightImagesFolder = Environment.GetEnvironmentVariable("LocalAppData") + @"\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
            var defaultDestinationFolder = "C:/Spotlight images";

            var imageProcessor = new ImageProcessor(spotlightImagesFolder, defaultDestinationFolder);
            imageProcessor.RetrieveImages();
        }
    }
}
