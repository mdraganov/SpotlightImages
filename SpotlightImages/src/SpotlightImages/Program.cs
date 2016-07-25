using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SpotlightImages
{
    public class Program
    {
        private const string SpotlightImagesLocation = @"\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";

        public static void Main(string[] args)
        {
            IConfigurationRoot config = null;
            string destinationPath = string.Empty;

            try
            {
                config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                destinationPath = config["destinationPath"];
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Missing appsettings.json");
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid appsettings.json");
            }

            Console.WriteLine("Saving spotlight images in: " + destinationPath);
            Console.Write("Would you like to change the location? y/n: ");
            var input = Console.ReadLine();

            while (input != "n" && input != "N" && input != "y" && input != "Y")
            {
                Console.Write("Invalid input. Please enter y or n: ");
                input = Console.ReadLine();
            }

            //var newLocation = string.Empty;

            if (input == "y" || input == "Y")
            {
                Console.Write("Please enter new location path: ");
                destinationPath = Console.ReadLine().Replace('\\', '/');

                config["destinationPath"] = destinationPath;
            }

            List<FileInfo> filesInDestination;
            try
            {
                filesInDestination = new DirectoryInfo(destinationPath).EnumerateFiles().ToList();
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid destination path. Press enter to exit.");
                Console.ReadLine();
                return;
            }

            var path = Environment.GetEnvironmentVariable("LocalAppData") + SpotlightImagesLocation;

            IEnumerable<FileInfo> files = new DirectoryInfo(path).EnumerateFiles();
            var copiedImagesCount = 0;

            foreach (var file in files)
            {
                if (filesInDestination.Any(x => x.Name == file.Name + ".jpg"))
                {
                    continue;
                }

                File.Copy(file.FullName, destinationPath + file.Name + ".jpg");
                copiedImagesCount++;
            }

            Console.WriteLine(copiedImagesCount + " images copied. Press enter to exit.");
            Console.ReadLine();
        }
    }
}
