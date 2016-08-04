using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SpotlightImages
{
    public class ProgramUI
    {
        public static void Main(string[] args)
        {
            var spotlightImagesFolder = Environment.GetEnvironmentVariable("LocalAppData") + @"\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
            var defaultDestinationFolder = "C:/Spotlight images";
                        
            Process.Start("explorer.exe", "C:\\Spotlight images");
            var imageProcessor = new ImageProcessor(spotlightImagesFolder, defaultDestinationFolder);
            imageProcessor.RetrieveImages();
        }
    }
}
