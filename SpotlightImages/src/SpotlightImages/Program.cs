using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace SpotlightImages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var path = Environment.GetEnvironmentVariable("LocalAppData") + @"\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";

            DirectoryInfo di = new DirectoryInfo(path);

            IEnumerable<FileInfo> files = di.EnumerateFiles();

            foreach (var file in files)
            {
                Console.WriteLine(file.FullName);
            }

            Console.ReadLine();
        }
    }
}
